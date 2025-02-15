using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> GetBookedSlotsAsync(AvailableSlotsRequest request)
        {
            // Get all bookings for the current day and field where the slot is booked (FlagBooked = true)
            var bookedSlots = await _context.Bookings
                .Where(b => b.Date.Date == request.Date.Date && b.FieldId == request.FieldId && b.FlagBooked == true)  // Only booked slots
                .Select(b => b.TimeSlot)  // Select the time slot
                .ToListAsync();  // Execute the query asynchronously

            return new Status
            {
                Code = "0000",
                Message = "Booked slots fetched successfully.",
                Data = bookedSlots
            };
        }


        // Reserve a booking
        public async Task<Status> ReserveBookingAsync(ReserveBookingRequest request)
        {
            // Step 1: Check if the user exists by their email in the Users table.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Status
                {
                    Code = "1001",
                    Message = "User does not exist.",
                    Data = null
                };
            }

            // Step 2: Check if the user has a valid FIT membership using the email.
            var fitMember = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == request.Email);
            if (fitMember == null)
            {
                return new Status
                {
                    Code = "1002",
                    Message = "User does not have a valid FIT membership.",
                    Data = null
                };
            }

            // Step 3: Check if the slot is already booked.
            var existingBooking = await _context.Bookings
                .Where(b => b.Date == request.Date && b.TimeSlot == request.TimeSlot && b.FieldId == request.FieldId && !b.FlagCanceled)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                return new Status
                {
                    Code = "1003",
                    Message = "The selected time slot is already booked.",
                    Data = null
                };
            }

            // Step 4: Calculate the end time based on the duration provided in the request.
            var endTime = request.Date.AddMinutes(request.Duration);

            // Step 5: Create the booking.
            var booking = new Booking
            {
                SportType = request.SportType,
                Date = request.Date,
                TimeSlot = request.TimeSlot,
                FieldId = request.FieldId,
                PaymentMethod = request.PaymentMethod, // Store the payment method type (Cash, PayPal)
                Amount = request.Amount,               // Store the booking amount
                FlagBooked = true,
                FlagCanceled = false,
                Email = request.Email,                 // Store the user's email with the booking
                EndTime = endTime                      // Store the calculated end time
            };

            // Step 6: Save the booking to the database.
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new Status
            {
                Code = "0000",
                Message = "Booking successful.",
                Data = booking
            };
        }

        // Fetch user's bookings
        public async Task<Status> GetUserBookingsAsync(GetUserBookingsRequest request)
        {
            var userBookings = await _context.Bookings
                .Where(b => b.Email == request.Email && !b.FlagCanceled)
                .ToListAsync();

            return new Status
            {
                Code = "0000",
                Message = "User bookings fetched successfully.",
                Data = userBookings
            };
        }

        // Cancel a booking
        public async Task<Status> CancelBookingAsync(CancelBookingRequest request)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId && b.Email == request.Email);

            if (booking == null)
            {
                return new Status
                {
                    Code = "1004",
                    Message = "Booking not found or the user is not authorized to cancel this booking.",
                    Data = null
                };
            }

            booking.FlagCanceled = true;
            await _context.SaveChangesAsync();

            return new Status
            {
                Code = "0000",
                Message = "Booking canceled successfully.",
                Data = booking
            };
        }

        // Reschedule a booking
        public async Task<Status> RescheduleBookingAsync(RescheduleBookingRequest request)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId && b.Email == request.Email);

            if (booking == null)
            {
                return new Status
                {
                    Code = "1005",
                    Message = "Booking not found or the user is not authorized to reschedule this booking.",
                    Data = null
                };
            }

            booking.Date = request.NewDate;
            booking.TimeSlot = request.NewTimeSlot;
            await _context.SaveChangesAsync();

            return new Status
            {
                Code = "0000",
                Message = "Booking rescheduled successfully.",
                Data = booking
            };
        }

        // Fetch booking history
        public async Task<Status> GetBookingHistoryAsync(BookingHistoryRequest request)
        {
            var bookingHistory = await _context.Bookings
                .Where(b => b.Email == request.Email)
                .ToListAsync();

            return new Status
            {
                Code = "0000",
                Message = "Booking history fetched successfully.",
                Data = bookingHistory
            };
        }

        // Fetch details of a specific booking
        public async Task<Status> GetBookingDetailsAsync(BookingDetailsRequest request)
        {
            var booking = await _context.Bookings.FindAsync(request.BookingId);

            if (booking == null)
            {
                return new Status
                {
                    Code = "1006",
                    Message = "Booking not found.",
                    Data = null
                };
            }

            return new Status
            {
                Code = "0000",
                Message = "Booking details fetched successfully.",
                Data = booking
            };
        }
    }
}
