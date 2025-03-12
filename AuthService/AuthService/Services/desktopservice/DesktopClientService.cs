﻿using AuthService.Bridge;
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


        public async Task<Status> GetAllUsersAsync()
        {
            var users = await _context.Users
                                       .Select(u => new
                                       {
                                           ClientName = u.Name,
                                           CustomerSurname = u.Surname,
                                           Email = u.Email,
                                           Telephone = u.Cell,

                                           Membership = _context.MembershipUsers
                                                                .Where(m => m.Email == u.Email)
                                                                .Select(m => new
                                                                {
                                                                    m.Id,
                                                                    m.CardNumber,
                                                                    FITCardExpiryDate = m.ExpiryDate,
                                                                    MedicalCertificateExpiryDate = m.MedicalCertificateDate,
                                                                    m.MedicalCertificatePath,
                                                                    m.FirstName,
                                                                    m.LastName,
                                                                    m.Gender,
                                                                    m.BirthDate,
                                                                    m.ProvinceOfBirth,
                                                                    m.MunicipalityOfBirth,
                                                                    m.TaxCode,
                                                                    m.Citizenship,
                                                                    m.ProvinceOfResidence,
                                                                    m.MunicipalityOfResidence,
                                                                    m.PostalCode,
                                                                    m.ResidentialAddress,
                                                                    m.PhoneNumber,
                                                                    m.PaymentMethod,
                                                                    m.Email,
                                                                    m.IsVerified
                                                                })
                                                                .FirstOrDefault(),

                                           PlayerType = _context.Bookings
                                                               .Where(b => b.Email == u.Email && b.FlagBooked)
                                                               .Select(b => b.SportType)
                                                               .FirstOrDefault() // Get the first sport type if multiple exist
                                       })
                                       .ToListAsync();

            if (users == null || !users.Any())
            {
                return new Status { Code = "1001", Message = "No users found", Data = null };
            }

            var formattedUsers = users.Select(user => new
            {
                user.ClientName,
                user.CustomerSurname,
                user.Email,
                user.Telephone,
                FITCardExpiryDate = user.Membership?.FITCardExpiryDate, // Nullable if membership doesn't exist
                MedicalCertificateExpiryDate = user.Membership?.MedicalCertificateExpiryDate, // Nullable
                MedicalCertificatePath = user.Membership?.MedicalCertificatePath,
                FirstName = user.Membership?.FirstName,
                LastName = user.Membership?.LastName,
                Gender = user.Membership?.Gender,
                BirthDate = user.Membership?.BirthDate,
                ProvinceOfBirth = user.Membership?.ProvinceOfBirth,
                MunicipalityOfBirth = user.Membership?.MunicipalityOfBirth,
                TaxCode = user.Membership?.TaxCode,
                Citizenship = user.Membership?.Citizenship,
                ProvinceOfResidence = user.Membership?.ProvinceOfResidence,
                MunicipalityOfResidence = user.Membership?.MunicipalityOfResidence,
                PostalCode = user.Membership?.PostalCode,
                ResidentialAddress = user.Membership?.ResidentialAddress,
                PhoneNumber = user.Membership?.PhoneNumber,
                PaymentMethod = user.Membership?.PaymentMethod,
                IsVerified = user.Membership?.IsVerified,
                PlayerType = user?.PlayerType // Nullable if no bookings exist
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
                                             Amount = b.Amount,              // Payment amount
                                             PaymentMethod = b.PaymentMethod // Payment method used
                                         })
                                         .OrderByDescending(b => b.Date) // Show latest bookings first
                                         .ToListAsync();

            if (!bookings.Any())
            {
                return new Status { Code = "1001", Message = "No booking history found", Data = null };
            }

            return new Status
            {
                Code = "0000",
                Message = "Customer booking history fetched successfully",
                Data = bookings
            };
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
                    IsVerified = request.IsVerified ?? false, // Default to false if not provided
                    FirstName = request.Name,  // Add Name to Membership table
                    LastName = request.Surname // Add Surname to Membership table
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
                                          .Where(b => b.Email == email && b.FlagBooked) // Ensure the booking is confirmed
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
                        FavoriteField = "",
                        CancelledBookings = 0,
                        LifetimeValueInEuros = 0
                    }
                };
            }

            var totalBookings = bookings.Count;
            var cancelledBookings = bookings.Count(b => b.FlagCanceled); // Check using FlagCanceled for cancellations
            var favoriteField = bookings
                .GroupBy(b => b.SportType)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            var totalAmount = bookings.Sum(b => b.Amount); // Assuming Amount is stored in the booking

            var uniqueMonths = bookings.Select(b => b.Date.Month).Distinct().Count();
            var averageMonthlyBookings = uniqueMonths > 0 ? totalBookings / uniqueMonths : 0;

            var lifetimeValueInEuros = totalAmount; // Assuming Amount is in local currency and converting to Euros as needed

            return new Status
            {
                Code = "0000",
                Message = "Customer statistics fetched successfully",
                Data = new
                {
                    TotalBookings = totalBookings,
                    AverageMonthlyBookings = averageMonthlyBookings,
                    FavoriteField = favoriteField,
                    CancelledBookings = cancelledBookings,
                    LifetimeValueInEuros = lifetimeValueInEuros
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
