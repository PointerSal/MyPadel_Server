using AuthService.Bridge;
using AuthService.Interfaces.DesktopInterface;
using AuthService.Model;
using AuthService.Models;
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


        public async Task<Status> GetAllUsersAsync()
        {
            var users = await _context.Users
                .AsNoTracking()  // Disable change tracking to improve performance
                .Select(u => new
                {
                    u.Name,
                    u.Surname,
                    u.Email,
                    u.Cell,
                    FITCardExpiryDate = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.ExpiryDate)
                        .FirstOrDefault(),
                    MedicalCertificateExpiryDate = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.MedicalCertificateDate)
                        .FirstOrDefault(),
                    FirstName = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.FirstName)
                        .FirstOrDefault(),
                    LastName = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.LastName)
                        .FirstOrDefault(),
                    Gender = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.Gender)
                        .FirstOrDefault(),
                    BirthDate = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.BirthDate)
                        .FirstOrDefault(),
                    ProvinceOfBirth = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.ProvinceOfBirth)
                        .FirstOrDefault(),
                    MunicipalityOfBirth = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.MunicipalityOfBirth)
                        .FirstOrDefault(),
                    TaxCode = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.TaxCode)
                        .FirstOrDefault(),
                    Citizenship = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.Citizenship)
                        .FirstOrDefault(),
                    ProvinceOfResidence = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.ProvinceOfResidence)
                        .FirstOrDefault(),
                    MunicipalityOfResidence = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.MunicipalityOfResidence)
                        .FirstOrDefault(),
                    PostalCode = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.PostalCode)
                        .FirstOrDefault(),
                    ResidentialAddress = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.ResidentialAddress)
                        .FirstOrDefault(),
                    PhoneNumber = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.PhoneNumber)
                        .FirstOrDefault(),
                    PaymentMethod = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.PaymentMethod)
                        .FirstOrDefault(),
                    IsVerified = _context.MembershipUsers
                        .Where(m => m.Email == u.Email)
                        .Select(m => m.IsVerified)
                        .FirstOrDefault(),
                    PlayerType = _context.Bookings
                        .Where(b => b.Email == u.Email && b.FlagBooked)
                        .Select(b => b.SportType)
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (users == null || !users.Any())
            {
                return new Status { Code = "1001", Message = "No users found", Data = null };
            }

            var formattedUsers = users.Select(user => new
            {
                user.Name,
                user.Surname,
                user.Email,
                user.Cell,
                FITCardExpiryDate = user.FITCardExpiryDate,
                MedicalCertificateExpiryDate = user.MedicalCertificateExpiryDate,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                ProvinceOfBirth = user.ProvinceOfBirth,
                MunicipalityOfBirth = user.MunicipalityOfBirth,
                TaxCode = user.TaxCode,
                Citizenship = user.Citizenship,
                ProvinceOfResidence = user.ProvinceOfResidence,
                MunicipalityOfResidence = user.MunicipalityOfResidence,
                PostalCode = user.PostalCode,
                ResidentialAddress = user.ResidentialAddress,
                PhoneNumber = user.PhoneNumber,
                PaymentMethod = user.PaymentMethod,
                IsVerified = user.IsVerified,
                PlayerType = user.PlayerType
            }).ToList();

            return new Status
            {
                Code = "0000",
                Message = "Users fetched successfully",
                Data = formattedUsers
            };
        }



        // Fetch customer booking history based on email
        public async Task<Status> GetCustomerBookingHistoryAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new Status { Code = "1002", Message = "Invalid email provided", Data = null };
            }

            var bookings = await _context.Bookings
                                         .Where(b => b.Email == email && b.FlagBooked) // Fetch only booked records
                                         .Select(b => new
                                         {
                                             Date = b.Date,                  // Booking Date
                                             Field = b.SportType,            // Sport Type
                                             StartingHour = b.Date,          // Start Time from Date field
                                             Duration = b.TimeSlot, // Calculate duration in minutes
                                             Amount = b.Amount,
                                             BookingDate = b.BookingDate,
                                             PaymentMethod = b.PaymentMethod // Payment method used
                                         })
                                         .OrderByDescending(b => b.Date) // Show latest bookings first
                                         .ToListAsync();

            if (!bookings.Any())
            {
                return new Status {
                    Code = "0000",
                    Message = "No bookings found for the given User",
                    Data = new List<object>()
                };
            }

            return new Status
            {
                Code = "0000",
                Message = "Customer booking history fetched successfully",
                Data = bookings
            };
        }

        public async Task<Status> RenewMembershipAsync(string email)
        {
            var membershipUser = await _context.MembershipUsers
                .FirstOrDefaultAsync(m => m.Email == email);

            if (membershipUser == null)
            {
                return new Status { Code = "1001", Message = "Membership not found for the provided email", Data = null };
            }

            // Update ExpiryDate and IsVerified
            membershipUser.ExpiryDate = null;
            membershipUser.IsVerified = false;

            await _context.SaveChangesAsync();  // Save changes to the database

            return new Status
            {
                Code = "0000",
                Message = "Membership renewed successfully",
                Data = null
            };
        }
        // Search client by email and fetch relevant data
        public async Task<Status> SearchDesktopClientByEmailAsync(string email)
        {
            // Fetch client from User table and MembershipUser table by joining through email
            var client = await _context.Users
                            .AsNoTracking() // Disable change tracking
                            .Where(u => u.Email == email)
                            .Select(u => new
                            {
                                u.Name,
                                u.Surname,
                                u.Email,
                                u.Cell,
                                Membership = _context.MembershipUsers
                                                     .AsNoTracking() // Disable change tracking for MembershipUser
                                                     .Where(m => m.Email == u.Email)
                                                     .Select(m => new
                                                     {
                                                         m.CardNumber,
                                                         m.ExpiryDate
                                                     })
                                                     .FirstOrDefault(),
                                Bookings = _context.Bookings
                                                   .AsNoTracking() // Disable change tracking for Bookings
                                                   .Where(b => b.Email == u.Email && b.FlagBooked)
                                                   .Select(b => new
                                                   {
                                                       b.SportType,
                                                       b.Date,
                                                       b.TimeSlot
                                                   })
                                                   .ToList() // Convert to list for projection
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




        public async Task<Status> AddUserAsync(AddUserRequest request)
        {
            try
            {
                // Hash the password using BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create a new User object and save to Users table
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Password = hashedPassword,
                    IsMarketing = request.IsMarketing,
                    ProfilePicture = request.ProfilePicture
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Create a new MembershipUser object and save to MembershipUsers table
                var membershipUser = new MembershipUser
                {
                    Email = request.Email,
                    CardNumber = request.CardNumber,
                    ExpiryDate = request.ExpiryDate,
                    MedicalCertificateDate = request.MedicalCertificateDate,
                    MedicalCertificatePath = request.MedicalCertificatePath,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    BirthDate = request.BirthDate,
                    ProvinceOfBirth = request.ProvinceOfBirth,
                    MunicipalityOfBirth = request.MunicipalityOfBirth,
                    TaxCode = request.TaxCode,
                    Citizenship = request.Citizenship,
                    ProvinceOfResidence = request.ProvinceOfResidence,
                    MunicipalityOfResidence = request.MunicipalityOfResidence,
                    PostalCode = request.PostalCode,
                    ResidentialAddress = request.ResidentialAddress,
                    PhoneNumber = request.PhoneNumber,
                    PaymentMethod = request.PaymentMethod,
                    IsVerified = request.IsVerified
                };

                _context.MembershipUsers.Add(membershipUser);
                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "User added successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = $"Error adding user: {ex.Message}", Data = null };
            }
        }

        public async Task<Status> UpdateDesktopClientInformationAsync(DesktopClientUpdateRequest request)
            {
                var client = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (client == null)
                {
                    return new Status { Code = "1001", Message = "Client not found", Data = null };
                }

                // Optimized updating of fields, only update if not null or empty
                client.Name = string.IsNullOrEmpty(request.Name) ? client.Name : request.Name;
                client.Surname = string.IsNullOrEmpty(request.Surname) ? client.Surname : request.Surname;
                client.Cell = string.IsNullOrEmpty(request.Telephone) ? client.Cell : request.Telephone;

                // Update Name in MembershipUsers table as well
                var membership = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == request.Email);
                if (membership != null)
                {
                    // Update fields only if provided
                    membership.ExpiryDate = request.FitCardExpiryDate != null ? request.FitCardExpiryDate : membership.ExpiryDate;
                    membership.CardNumber = string.IsNullOrEmpty(request.CardNumber) ? membership.CardNumber : request.CardNumber; // Update if provided
                    membership.MedicalCertificatePath = string.IsNullOrEmpty(request.MedicalCertificate) ? membership.MedicalCertificatePath : request.MedicalCertificate;
                    membership.MedicalCertificateDate = request.MedicalCertificateDate != null ? request.MedicalCertificateDate : membership.MedicalCertificateDate;

                // Update IsVerified if provided in the request
                if (request.IsVerified.HasValue)
                    {
                        membership.IsVerified = request.IsVerified.Value; // Set IsVerified from the request
                    }

                    // Update Name in MembershipUsers table as well
                    membership.FirstName = string.IsNullOrEmpty(request.Name) ? membership.FirstName : request.Name;
                    membership.LastName = string.IsNullOrEmpty(request.Surname) ? membership.LastName : request.Surname;
                }
                else
                {
                    var newMembership = new MembershipUser
                    {
                        Email = request.Email,
                        CardNumber = request.CardNumber,
                        ExpiryDate = request.FitCardExpiryDate,
                        IsVerified = request.IsVerified ?? false, 
                        FirstName = request.Name,   
                        LastName = request.Surname,
                        MedicalCertificatePath = request.MedicalCertificate ?? null,
                        MedicalCertificateDate = request.MedicalCertificateDate
                    };
                    _context.MembershipUsers.Add(newMembership);
                }

                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "Client information updated successfully", Data = null };
            }


        public async Task<Status> GetCustomerStatisticsAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new Status { Code = "1002", Message = "Invalid email provided", Data = null };
            }

            // Fetch all relevant bookings for the user
            var bookings = await _context.Bookings
                                          .Where(b => b.Email == email && b.FlagBooked)
                                          .ToListAsync();

            if (!bookings.Any())
            {
                return new Status
                {
                    Code = "0000",
                    Message = "Customer statistics fetched successfully",
                    Data = new
                    {
                        TotalBookings = 0,
                        AverageMonthlyBookings = 0,
                        AverageMonthlyPrice = 0,
                        FavoriteField = "",
                        CancelledBookings = 0,
                        LifetimeValueInEuros = 0,
                        CurrentMonthTotal = 0
                    }
                };
            }

            var totalBookings = bookings.Count;
            var cancelledBookings = bookings.Count(b => b.FlagCanceled);
            var favoriteField = bookings
                .GroupBy(b => b.SportType)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            var totalAmount = bookings.Sum(b => b.Amount);

            // Unique months in which bookings were made
            var uniqueMonths = bookings.Select(b => b.Date.Month).Distinct().Count();
            var averageMonthlyBookings = uniqueMonths > 0 ? totalBookings / uniqueMonths : 0;

            // Average monthly price (Total Amount / Unique Months)
            var averageMonthlyPrice = uniqueMonths > 0 ? totalAmount / uniqueMonths : 0;

            var lifetimeValueInEuros = totalAmount; // Assuming Amount is in local currency and converting to Euros as needed

            // Current month booking total (using the current date to get this month)
            var currentMonth = DateTime.Now.Month;
            var currentMonthBookings = bookings.Where(b => b.Date.Month == currentMonth);
            var currentMonthTotal = currentMonthBookings.Sum(b => b.Amount);

            return new Status
            {
                Code = "0000",
                Message = "Customer statistics fetched successfully",
                Data = new
                {
                    TotalBookings = totalBookings,
                    AverageMonthlyBookings = averageMonthlyBookings,
                    AverageMonthlyPrice = averageMonthlyPrice,
                    FavoriteField = favoriteField,
                    CancelledBookings = cancelledBookings,
                    LifetimeValueInEuros = lifetimeValueInEuros,
                    CurrentMonthTotal = currentMonthTotal
                }
            };
        }


        public async Task<Status> DeleteUserAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new Status { Code = "1002", Message = "Invalid email provided", Data = null };
            }

            // Start a transaction to ensure all related data is deleted
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Delete user from User table
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        return new Status { Code = "1001", Message = "User not found", Data = null };
                    }

                    // Delete related records from MembershipUsers table
                    var membership = await _context.MembershipUsers.FirstOrDefaultAsync(m => m.Email == email);
                    if (membership != null)
                    {
                        _context.MembershipUsers.Remove(membership);
                    }

                    // Delete related records from Bookings table
                    var bookings = await _context.Bookings.Where(b => b.Email == email).ToListAsync();
                    if (bookings.Any())
                    {
                        _context.Bookings.RemoveRange(bookings);
                    }

                    // Now delete the user
                    _context.Users.Remove(user);

                    // Commit all deletions
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new Status { Code = "0000", Message = "User deleted successfully", Data = null };
                }
                catch (Exception ex)
                {
                    // Rollback if any error occurs
                    await transaction.RollbackAsync();
                    return new Status { Code = "1003", Message = $"Error deleting user: {ex.Message}", Data = null };
                }
            }
        }


    }
}
