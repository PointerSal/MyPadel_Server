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
                var bookings = await (from b in _context.Bookings
                                      join u in _context.Users on b.Email equals u.Email into userEmailJoin
                                      from u in userEmailJoin.DefaultIfEmpty()
                                      join p in _context.Users on b.PhoneNumber equals p.Cell into userPhoneJoin
                                      from p in userPhoneJoin.DefaultIfEmpty()
                                      join c in _context.CourtSports on b.FieldId equals c.Id // Join with CourtSports to get FieldName
                                      where b.Date.Date == date.Date
                                      select new
                                      {
                                          b.Id,
                                          b.Date,
                                          b.EndTime,
                                          b.SportType,
                                          b.FieldId,
                                          c.FieldName,
                                          b.PaymentMethod,
                                          b.Amount,
                                          FirstName = u != null ? u.Name : p.Name,  // Use Name from either email or phone number
                                          LastName = u != null ? u.Surname : p.Surname, // Use Surname from either email or phone number
                                          Duration = b.EndTime.Subtract(b.Date).TotalMinutes,
                                          Status = b.FlagArchived ? "Archived" : b.FlagCanceled ? "Canceled" : b.FlagBooked ? "Booked" : "Unknown"
                                      })
                 .ToListAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Bookings fetched successfully",
                    Data = bookings
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
