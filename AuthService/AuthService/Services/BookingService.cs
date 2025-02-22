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
            // Get all bookings for the requested field where the slot is booked (FlagBooked = true)
            var bookedSlots = await _context.Bookings
                .Where(b => b.FieldId == request.FieldId && b.FlagBooked && b.Date.Date >= request.Date.Date) // Ensure we're checking from today's date onward
                .OrderBy(b => b.Date) // Sort by booking date/time
                .Select(b => new
                {
                    StartTime = b.Date.ToString("yyyy-MM-dd HH:mm:ss"),  // Start time in the format of 'yyyy-MM-dd HH:mm:ss'
                    EndTime = b.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), // End time in the same format
                    Date = b.Date.Date.ToString("yyyy-MM-dd") // Just to group by date (if needed)
                })
                .ToListAsync();

            // Group the bookings by date
            var groupedSlots = bookedSlots
                .GroupBy(b => b.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Slots = g.Select(slot => new
                    {
                        slot.StartTime,
                        slot.EndTime
                    }).ToList()
                })
                .ToList();

            return new Status
            {
                Code = "0000",
                Message = "Booked slots fetched successfully.",
                Data = groupedSlots
            };
        }


        public async Task<Status> ReserveBookingAsync(ReserveBookingRequest request)
        {
             var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return new Status { Code = "1001", Message = "User does not exist." };

            var fitMember = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == request.Email);
            if (fitMember == null)
                return new Status { Code = "1002", Message = "User does not have a valid FIT membership." };

             var startTime = request.Date;
            var endTime = startTime.AddMinutes(request.Duration);

            var conflictingBooking = await _context.Bookings
                .Where(b => b.FieldId == request.FieldId && !b.FlagCanceled
                            && ((b.Date < endTime && b.EndTime > startTime) || (b.Date < startTime && b.EndTime > startTime)))
                .FirstOrDefaultAsync();

            if (conflictingBooking != null)
                return new Status { Code = "1003", Message = "The selected time slot is already booked." };

             bool isArchived = startTime < DateTime.Now;

             var booking = new Booking
            {
                SportType = request.SportType,
                Date = startTime,
                TimeSlot = request.TimeSlot,
                FieldId = request.FieldId,
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                FlagBooked = true,
                FlagCanceled = false,
                FlagArchived = isArchived,
                Email = request.Email,
                EndTime = endTime,
                PaymentStatus = null,  
                PaymentId = ""
             };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Booking successful.", Data = booking };
        }

         public async Task<Status> GetUserBookingsAsync(GetUserBookingsRequest request)
        {
            var activeBookings = await _context.Bookings
                .Where(b => b.Email == request.Email && !b.FlagCanceled && b.Date >= DateTime.Now)
                .ToListAsync();

            return new Status
            {
                Code = "0000",
                Message = "User active bookings fetched successfully.",
                Data = activeBookings
            };
        }

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

         public async Task<Status> GetBookingHistoryAsync(BookingHistoryRequest request)
        {
            var activeBookings = await _context.Bookings
                .Where(b => b.Email == request.Email && !b.FlagCanceled && b.Date >= DateTime.Now)
                .ToListAsync();

            var archivedBookings = await _context.Bookings
                .Where(b => b.Email == request.Email && !b.FlagCanceled && b.Date < DateTime.Now && b.FlagArchived)
                .ToListAsync();

            var canceledBookings = await _context.Bookings
                .Where(b => b.Email == request.Email && b.FlagCanceled)
                .ToListAsync();

            var bookingHistory = new
            {
                Active = activeBookings,
                Archived = archivedBookings,
                Canceled = canceledBookings
            };

            return new Status
            {
                Code = "0000",
                Message = "Booking history fetched successfully.",
                Data = bookingHistory
            };
        }

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
