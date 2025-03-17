using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.desktopservice
{
    public class DesktopBookingService : IDesktopBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStripeService _stripeService;
        private readonly EmailService _emailService;

        public DesktopBookingService(ApplicationDbContext context, IStripeService stripeService, EmailService emailService)
        {
            _context = context;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        public async Task<Status> GetBookingsByDateAsync(DateTime date)
        {
            try
            {
                // Step 1: Fetch bookings for the given date using AsNoTracking() for performance improvement
                var bookings = await _context.Bookings
                    .AsNoTracking()  // Prevent entity tracking for read-only operations
                    .Where(b => b.Date.Date == date.Date)
                    .ToListAsync();

                // If no bookings are found, return early
                if (!bookings.Any())
                {
                    return new Status
                    {
                        Code = "1002",
                        Message = "No bookings found for the given date",
                        Data = null
                    };
                }

                // Step 2: Process each booking and check against CourtSports table
                var result = new List<object>();

                // Fetch court sport data with AsNoTracking() as we only need read access
                var courtSports = await _context.CourtSports
                    .AsNoTracking()
                    .ToListAsync();

                // Fetch user data with AsNoTracking() to improve performance
                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var booking in bookings)
                {
                    // Find matching CourtSports record based on SportType and FieldId
                    var courtSport = courtSports
                        .FirstOrDefault(c => c.SportsName == booking.SportType && c.FieldType == booking.FieldId.ToString());

                    // Find user details (either by email or phone number)
                    var user = users.FirstOrDefault(u => u.Email == booking.Email);
                    var phoneUser = users.FirstOrDefault(p => p.Cell == booking.PhoneNumber);

                    if (courtSport != null)
                    {
                        result.Add(new
                        {
                            booking.Id,
                            booking.Date,
                            booking.EndTime,
                            booking.SportType,
                            booking.FieldId,
                            courtSport.FieldName,
                            booking.PaymentMethod,
                            booking.Amount,
                            FirstName = user?.Name ?? phoneUser?.Name ?? "Unknown",
                            LastName = user?.Surname ?? phoneUser?.Surname ?? "Unknown",
                            Duration = booking.EndTime.Subtract(booking.Date).TotalMinutes,
                            Status = booking.FlagArchived ? "Archived" :
                                     booking.FlagCanceled ? "Canceled" :
                                     booking.FlagBooked ? "Booked" : "Unknown"
                        });
                    }
                }

                // Step 3: Return the results
                return new Status
                {
                    Code = "0000",
                    Message = "Bookings fetched successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1001",
                    Message = "Error fetching bookings",
                    Data = ex.Message
                };
            }
        }


        public async Task<Status> CancelBookingAsync(int bookingId)
        {
            try
            {
                // Fetch the booking based on the ID
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    return new Status { Code = "1002", Message = "Booking not found" };
                }

                // Cancel the booking
                booking.FlagBooked = false;       
                booking.FlagArchived = false;
                booking.FlagCanceled = true;
                await _context.SaveChangesAsync();

                // Process refund if payment exists
                if (!string.IsNullOrEmpty(booking.PaymentId))
                {
                    var refundResult = await _stripeService.ProcessRefund(booking.PaymentId, "Booking canceled");
                    if (refundResult.Code != "0000")
                    {
                        return new Status { Code = "1003", Message = "Refund failed" };
                    }
                }

                return new Status { Code = "0000", Message = "Booking canceled successfully" };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = "Error canceling booking", Data = ex.Message };
            }
        }


        public async Task<Status> ReserveBookingAsync(DesktopReserveBookingRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Cell == request.PhoneNumber);

            if (user == null)
                return new Status { Code = "1001", Message = "User does not exist." };

            //var fitMember = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == request.Email);
            //if (fitMember == null || !fitMember.IsVerified || fitMember.ExpiryDate < DateTime.Now)
            //{
            //    return new Status { Code = "1002", Message = "User does not have a valid, verified FIT membership or the membership has expired." };
            //}


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
     .Where(b => b.FieldId == request.FieldId && b.SportType == request.SportType
                 && !b.FlagCanceled
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
                PhoneNumber = request.PhoneNumber,
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


    }
}
