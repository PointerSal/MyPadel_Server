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

        public DesktopBookingService(ApplicationDbContext context, IStripeService stripeService)
        {
            _context = context;
            _stripeService = stripeService;
        }

        public async Task<Status> GetBookingsByDateAsync(DateTime date)
        {
            try
            {
                // Fetch bookings based on the date
                var bookings = await (from b in _context.Bookings
                                      join u in _context.Users on b.Email equals u.Email
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
                                          FirstName = u.Name,
                                          LastName = u.Surname,
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


        public async Task<Status> ReserveBookingAsync(ReserveBookingRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return new Status { Code = "1001", Message = "User does not exist." };

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


    }
}
