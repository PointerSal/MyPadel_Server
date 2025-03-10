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
        private readonly EmailService _emailService;
        private readonly IStripeService _stripeService;

        public BookingService(ApplicationDbContext context, EmailService emailService, IStripeService stripeService)
        {   
            _context = context;
            _emailService = emailService;
            _stripeService = stripeService;
        }

        public async Task<Status> GetBookedSlotsAsync(AvailableSlotsRequest request)
        {
            // Fetch all court fields where the SportsName matches
            var courts = await _context.CourtSports
                .Where(c => c.SportsName == request.SportsName)
                .Select(c =>new
                {
                    c.FieldName,
                    c.FieldType
                }) // Get the field names
                .ToListAsync();

            if (!courts.Any())
            {
                return new Status
                {
                    Code = "1001",
                    Message = "No courts found for the given sport name.",
                    Data = null
                };
            }

            // Fetch bookings for the retrieved court fields
            var bookedSlots = await _context.Bookings
                .Where(b => request.SportsName.Contains(b.SportType) && b.FlagBooked && b.Date.Date >= request.Date.Date)
                .OrderBy(b => b.Date)
                .Select(b => new
                {
                    FieldName = b.SportType, // Here, FieldName is stored in SportType
                    StartTime = b.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = b.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Date = b.Date.ToString("yyyy-MM-dd"),
                    FieldId = b.FieldId
                })
                .ToListAsync();

            // Group bookings by FieldName
            var groupedSlots = courts
                .Select(field => new
                {
                    FieldName = field.FieldName,
                    Slots = bookedSlots
                        .Where(slot => slot.FieldId.ToString() == field.FieldType) // Filter bookings for this field
                        .Select(slot => new
                        {
                            slot.StartTime,
                            slot.EndTime
                        })
                        .ToList()
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

            // Check if the booking date is in the past
            if (startTime.Date == DateTime.Now.Date) // Same day, check time
            {
                if (startTime < DateTime.Now) // If the time is in the past, reject
                {
                    return new Status { Code = "1007", Message = "You cannot book a slot for a past time today." };
                }
            }
            else if (startTime < DateTime.Now) // Future date, but in the past
            {
                return new Status { Code = "1007", Message = "You cannot book a slot for a past date." };
            }

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
            // Send confirmation email to the user
            string messageBody = $@"
        Dear {user.Name} {user.Surname},
        
        Your booking for {request.SportType} has been successfully made!
        
        Booking Details:
        Date: {startTime.ToString("MMMM dd, yyyy")}
        Time: {startTime.ToString("hh:mm tt")}
        Duration: {request.Duration} minutes
        Field: {request.FieldId}
        Payment Method: {request.PaymentMethod}
        Amount: {request.Amount}
        
        Thank you for booking with us!
    ";

            bool emailSent = _emailService.SendEmail(user.Email, "Booking Confirmation", messageBody);

            if (!emailSent)
            {
                return new Status { Code = "1005", Message = "Failed to send booking confirmation email.", Data = null };
            }

            return new Status { Code = "0000", Message = "Booking successful. A confirmation email has been sent.", Data = booking };

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

            // Check if the booking is archived
            if (booking.FlagArchived)
            {
                return new Status
                {
                    Code = "1005",
                    Message = "This booking is archived and cannot be canceled.",
                    Data = null
                };
            }

            // Check if the booking is within 24 hours from the current time
            if (booking.Date <= DateTime.Now.AddHours(24))
            {
                // If within 24 hours, cancel the booking but don't process the refund
                booking.FlagCanceled = true;
                await _context.SaveChangesAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Booking canceled successfully. No refund will be issued for cancellations within 24 hours.",
                    Data = booking
                };
            }

            booking.FlagCanceled = true;
            await _context.SaveChangesAsync();

            // Check for a payment ID and process the refund
            if (!string.IsNullOrEmpty(booking.PaymentId))
            {
                var refundResult = await _stripeService.ProcessRefund(booking.PaymentId, "Booking canceled");
                if (refundResult.Code != "0000")
                {
                    return new Status
                    {
                        Code = "1006",
                        Message = "Refund failed during cancellation.",
                        Data = refundResult.Message
                    };
                }
            }

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
