using AuthService.Bridge;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.desktopservice
{
    public class DesktopBookingService : IDesktopBookingService
    {
        private readonly ApplicationDbContext _context;

        public DesktopBookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> GetAllBookingsAsync()
        {
            try
            {
                // Join Booking with User Table based on Email to fetch details
                var bookings = await (from b in _context.Bookings
                                      join u in _context.Users on b.Email equals u.Email
                                      select new
                                      {
                                          b.Id,
                                          b.Date,
                                          b.EndTime,
                                          b.SportType,
                                          b.FieldId,
                                          b.PaymentMethod,
                                          b.Amount,
                                          FirstName = u.Name,
                                          LastName = u.Surname,
                                          Duration = b.EndTime.Subtract(b.Date).TotalMinutes
                                      })
                                      .ToListAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Bookings fetched successfully",
                    Data = bookings
                };
            }
            catch (System.Exception ex)
            {
                return new Status
                {
                    Code = "1001",
                    Message = "Error fetching bookings",
                    Data = ex.Message
                };
            }
        }
    }
}
