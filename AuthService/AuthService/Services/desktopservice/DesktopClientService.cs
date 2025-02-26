using AuthService.Bridge;
using AuthService.Interfaces.DesktopInterface;
using AuthService.Model;
using AuthService.Models.DesktopModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthService.Services.DesktopService
{
    public class DesktopClientService : IDesktopClientService
    {
        private readonly ApplicationDbContext _context;

        public DesktopClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Search client by email and fetch relevant data
        public async Task<Status> SearchDesktopClientByEmailAsync(string email)
        {
            // Fetch client from User table and MembershipUser table by joining through email
            var client = await _context.Users
                                       .Where(u => u.Email == email) // Search by email in the User table
                                       .Select(u => new
                                       {
                                           u.Name,
                                           u.Surname,
                                           u.Email,
                                           u.Cell,
                                           Membership = _context.MembershipUsers
                                                               .Where(m => m.Email == u.Email)
                                                               .Select(m => new
                                                               {
                                                                   m.CardNumber,
                                                                   m.ExpiryDate // ExpiryDate from MembershipUser table
                                                               })
                                                               .FirstOrDefault(), // Get the first membership matching the email
                                           Bookings = _context.Bookings
                                                              .Where(b => b.Email == u.Email && b.FlagBooked) // Only fetch booked bookings
                                                              .Select(b => new
                                                              {
                                                                  b.SportType, // Player type related to SportType
                                                                  b.Date,
                                                                  b.TimeSlot
                                                              })
                                       })
                                       .FirstOrDefaultAsync();

            if (client == null)
            {
                return new Status { Code = "1001", Message = "Client not found", Data = null };
            }

            return new Status
            {
                Code = "0000",
                Message = "Client details fetched successfully",
                Data = new
                {
                    client.Name,
                    client.Surname,
                    client.Email,
                    client.Cell,
                    client.Membership.CardNumber, // Get membership card number
                    client.Membership.ExpiryDate, // Display membership expiry date
                    Bookings = client.Bookings.Select(b => new
                    {
                        b.SportType, // Displaying SportType for the client related to player type
                        b.Date,
                        b.TimeSlot
                    }).ToList()
                }
            };
        }

        // Update client information

        public async Task<Status> UpdateDesktopClientInformationAsync(DesktopClientUpdateRequest request)
        {
            var client = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (client == null)
            {
                return new Status { Code = "1001", Message = "Client not found", Data = null };
            }

            // Update only the User fields that are relevant to the client search results
            client.Name = request.Name;
            client.Surname = request.Surname;
            client.Cell = request.Telephone; // Assuming the Telephone is used to update the Cell in User table

            // Update Membership information (for example, updating the expiry date)
            var membership = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == request.Email);
            if (membership != null)
            {
              //  membership.CardNumber = request.CardNumber ?? membership.CardNumber; // Only update if provided
                membership.ExpiryDate = request.FitCardExpiryDate; // Update Expiry Date
            }
            else
            {
                // If the MembershipUser doesn't exist, you can choose to add a new one
                var newMembership = new MembershipUser
                {
                    Email = request.Email,
                   // CardNumber = request.CardNumber,
                    ExpiryDate = request.FitCardExpiryDate
                };
                _context.MembershipUsers.Add(newMembership);
            }

            // Only update bookings if necessary, no changes are specified for bookings in the request.
            // If you want to update something in the bookings, uncomment and adjust this section as needed.
            // var bookings = await _context.Bookings.Where(b => b.Email == request.Email).ToListAsync();
            // foreach (var booking in bookings)
            // {
            //     booking.SportType = request.SportType; // Example: Update SportType in bookings
            // }

            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Client information updated successfully", Data = null };
        }



    }
}
