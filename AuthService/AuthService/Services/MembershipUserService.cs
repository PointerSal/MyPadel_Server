using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class MembershipUserService : IMembershipUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MembershipUserService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
      
        public async Task<Status> RegisterMembershipUser(MembershipUserRequest request)
        {
            try
            {
                var membershipUser = new MembershipUser
                {
                    CardNumber = request.CardNumber,
                    ExpiryDate = request.ExpiryDate,
                    MedicalCertificateDate = request.MedicalCertificateDate,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    BirthDate = request.BirthDate,
                    Address = request.Address,
                    PostalCode = request.PostalCode,
                    Municipality = request.Municipality,
                    PaymentMethod = request.PaymentMethod,
                    Email = request.Email,
                    MedicalCertificatePath = request.MedicalCertificate
                };

                _context.MembershipUsers.Add(membershipUser);
                await _context.SaveChangesAsync();

                var user = await _context.Users
                                          .Where(u => u.Email == request.Email)
                                          .FirstOrDefaultAsync();

                if (user != null)
                {
                    user.IsFitMember = true;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                return new Status { Code = "0000", Message = "Registration successful", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = "Error registering membership user", Data = ex.Message };
            }
        }

        public async Task<Status> AlreadyFitMember(FitMembershipRequest request)
        {
            try
            {
                // Try to find the existing membership user by email
                var fitMembership = await _context.MembershipUsers
                                                  .FirstOrDefaultAsync(m => m.Email == request.Email);

                // If no user is found, create a new membership user with only the necessary fields
                if (fitMembership == null)
                {
                    fitMembership = new MembershipUser
                    {
                        Email = request.Email,
                        CardNumber = request.MembershipNumber,
                        ExpiryDate = request.ExpiryDate,
                        MedicalCertificateDate = request.MedicalCertificateDate,
                        MedicalCertificatePath = request.MedicalCertificate, // Directly use the Base64 string
                        FirstName = "Null",
                        LastName = "Null",
                        Gender = "Null",
                        BirthDate = default(DateTime),
                        Address = "Null",
                        PostalCode = "Null",
                        Municipality = "Null",
                        PaymentMethod = "Null"
                    };

                    _context.MembershipUsers.Add(fitMembership);
                }
                else
                {
                    // Update the fields if the user exists
                    fitMembership.CardNumber = request.MembershipNumber ?? fitMembership.CardNumber;
                    fitMembership.ExpiryDate = request.ExpiryDate != default(DateTime) ? request.ExpiryDate : fitMembership.ExpiryDate;
                    fitMembership.MedicalCertificateDate = request.MedicalCertificateDate != default(DateTime) ? request.MedicalCertificateDate : fitMembership.MedicalCertificateDate;
                    fitMembership.MedicalCertificatePath = request.MedicalCertificate ?? fitMembership.MedicalCertificatePath; // Update certificate if provided
                }

                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "FIT Membership updated successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = "Error updating FIT Membership", Data = ex.Message };
            }
        }
        // Helper method to save the medical certificate
        private async Task<string> SaveMedicalCertificate(IFormFile certificate)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(certificate.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "medical_certificates", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await certificate.CopyToAsync(stream);
            }
            return "/medical_certificates/" + fileName;
        }

        public async Task<Status> GetExpiryDateByEmail(string email)
        {
            try
            {
                // Find the user by email
                var user = await _context.MembershipUsers
                                          .Where(u => u.Email == email)
                                          .FirstOrDefaultAsync();

                if (user != null)
                {
                    return new Status
                    {
                        Code = "0000",
                        Message = "Expiry date fetched successfully",
                        Data = new
                        {
                            ExpiryDate = user.ExpiryDate
                        }
                    };
                }

                return new Status
                {
                    Code = "1001",
                    Message = "User not found",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1002",
                    Message = "Error retrieving expiry date",
                    Data = ex.Message
                };
            }
        }

        public async Task<Status> GetCardDetailsByEmail(string email)
        {
            try
            {
                // Get membership details using email from the MembershipUsers table
                var membershipUser = await _context.MembershipUsers
                                                    .Where(m => m.Email == email)
                                                    .FirstOrDefaultAsync();

                if (membershipUser != null)
                {
                    return new Status
                    {
                        Code = "0000",
                        Message = "Card details fetched successfully",
                        Data = new
                        {
                            CardNumber = membershipUser.CardNumber,
                            ExpiryDate = membershipUser.ExpiryDate
                        }
                    };
                }

                return new Status
                {
                    Code = "1001",
                    Message = "Membership not found",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1003",
                    Message = "Error retrieving card details",
                    Data = ex.Message
                };
            }
        }
    }
}
