using AuthService.Model;
using AuthService.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Bridge;

namespace AuthService.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch available slots
        public async Task<Status> GetAvailableSlotsAsync(AvailableSlotsRequest request)
        {
            var availableSlots = await _context.Bookings
                .Where(b => b.Date == request.Date && b.FieldId == request.FieldId && !b.FlagBooked)
                .Select(b => b.TimeSlot)
                .ToListAsync();

            return new Status
            {
                Code = "0000",
                Message = "Available slots fetched successfully.",
                Data = availableSlots
            };
        }

        // Reserve a booking
        public async Task<Status> ReserveBookingAsync(ReserveBookingRequest request)
        {
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

            var fitMember = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.CardNumber == request.PaymentMethod);
            if (fitMember == null)
            {
                return new Status
                {
                    Code = "1002",
                    Message = "User does not have a valid FIT membership.",
                    Data = null
                };
            }

            // Check if the slot is available
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

            var booking = new Booking
            {
                SportType = request.SportType,
                Date = request.Date,
                TimeSlot = request.TimeSlot,
                FieldId = request.FieldId,
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                FlagBooked = true,
                FlagCanceled = false,
                Email = request.Email // Save the user's email with the booking
            };

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
