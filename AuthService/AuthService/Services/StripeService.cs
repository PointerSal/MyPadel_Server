﻿using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Model.Stripe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;
        private readonly string _successUrl;
        private readonly string _cancelUrl;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public StripeService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _secretKey = configuration["Stripe:SecretKey"];
            _successUrl = configuration["Stripe:SuccessUrl"];
            _cancelUrl = configuration["Stripe:CancelUrl"];
            StripeConfiguration.ApiKey = _secretKey;
            _context = context;
        }

        // Step 1: Create Checkout Session
        public async Task<Status> CreateCheckoutSession(PaymentRequest request)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = request.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Payment for Booking"
                            },
                            UnitAmount = (long)(request.Amount * 100) // Amount in cents
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{_successUrl}?session_id={{CHECKOUT_SESSION_ID}}",  // Direct to success URL
                CancelUrl = _cancelUrl,  // Direct to cancel URL
                Metadata = new Dictionary<string, string>
                {
                    { "Email", request.Email },  // Store the email in metadata
                    { "Amount", request.Amount.ToString() }  // Store the amount in metadata
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // Step 2: Find the correct booking based on Email and Amount
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Email == request.Email && b.Amount == request.Amount && string.IsNullOrEmpty(b.PaymentId));  // Match email and amount

            if (booking != null)
            {
                // Save payment status as Pending initially
                booking.PaymentStatus = "Pending";
                await _context.SaveChangesAsync();
            }

            return new Status
            {
                Code = "0000",
                Message = "Checkout session created successfully.",
                Data = new { sessionUrl = session.Url }
            };
        }

        // Step 3: Complete Payment and Update Booking Status
        public async Task<bool> CompletePayment(string sessionId, string paymentStatus)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.PaymentStatus == "paid")
            {
                // Fetch the booking with the specified email and amount
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Email == session.Metadata["Email"] && b.Amount == session.AmountTotal / 100 && string.IsNullOrEmpty(b.PaymentId));  // Ensure the amount matches

                if (booking != null)
                {
                    booking.PaymentStatus = paymentStatus;  // Set status as Paid/Failed
                    booking.PaymentId = session.PaymentIntentId;  // Store the PaymentId from Stripe
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        // Step 4: Process Stripe Webhook for Events (Payment success/failure)
        public async Task ProcessWebhook(string json, string stripeSignature)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

                // Handle Payment Success Event
                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    // Attempt to get the Email metadata safely
                    if (paymentIntent.Metadata.TryGetValue("Email", out var email))
                    {
                        var booking = await _context.Bookings
                            .FirstOrDefaultAsync(b => string.IsNullOrEmpty(b.PaymentId) && b.Email == email);

                        if (booking != null)
                        {
                            booking.PaymentStatus = "Paid";
                            booking.PaymentId = paymentIntent.Id; // Set PaymentId from Stripe
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Email metadata not found for payment intent.");
                    }
                }

            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Webhook Error: {ex.Message}");
            }
        }


        public async Task<Status> ProcessRefund(string paymentIntentId, string refundRequestedBy)
        {
            try
            {
                // Step 1: Check if this PaymentIntentId already has a refund
                var existingRefund = await _context.Refunds
                    .FirstOrDefaultAsync(r => r.PaymentIntentId == paymentIntentId);

                if (existingRefund != null)
                {
                    return new Status { Code = "1004", Message = "Refund already processed for this transaction." };
                }

                // Step 2: Find the booking details
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.PaymentId == paymentIntentId);

                if (booking == null)
                {
                    return new Status { Code = "1005", Message = "Booking not found for the given Payment ID." };
                }

                // Step 3: Call Stripe Refund API
                var refundService = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Reason = RefundReasons.RequestedByCustomer
                };

                var refund = await refundService.CreateAsync(refundOptions);

                if (refund.Status == "succeeded")
                {
                    // Step 4: Save refund details in Refund table
                    var newRefund = new Model.Stripe.Refund
                    {
                        PaymentIntentId = paymentIntentId,
                        OriginalEmail = booking.Email, // Store booking email
                        RefundRequestedBy = refundRequestedBy, // Store the email from which refund request came
                        RefundAmount = booking.Amount,
                        RefundStatus = "Processed",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Refunds.Add(newRefund);
                    await _context.SaveChangesAsync();

                    // Step 5: Update booking status
                    booking.PaymentStatus = "Refunded";
                    await _context.SaveChangesAsync();

                    return new Status { Code = "0000", Message = "Refund successful." };
                }

                return new Status { Code = "1002", Message = "Refund failed." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refund Error: {ex.Message}");
                return new Status { Code = "1003", Message = "Refund process error." };
            }
        }




    }
}
