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
                    Email = request.Email // Storing email for filtering
                };

                // Save Medical Certificate File
                if (request.MedicalCertificate != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.MedicalCertificate.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "medical_certificates", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.MedicalCertificate.CopyToAsync(stream);
                    }

                    membershipUser.MedicalCertificatePath = "/medical_certificates/" + fileName;
                }

                _context.MembershipUsers.Add(membershipUser);
                await _context.SaveChangesAsync();


                // Update IsFitMember to true in the User table once the membership is registered
                var user = await _context.Users
                                          .Where(u => u.Email == request.Email)
                                          .FirstOrDefaultAsync();

                if (user != null)
                {
                    // Set IsFitMember to true and save the changes
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
                // Retrieve existing membership user by email
                var fitMembership = await _context.MembershipUsers
                                                  .Where(m => m.Email == request.Email) // Use email to identify the record
                                                  .FirstOrDefaultAsync();

                if (fitMembership == null)
                {
                    return new Status { Code = "1001", Message = "User not found", Data = null };
                }

                // Only update the fields that are provided
                if (!string.IsNullOrEmpty(request.MembershipNumber))
                    fitMembership.CardNumber = request.MembershipNumber;

                if (request.ExpiryDate != null)
                    fitMembership.ExpiryDate = request.ExpiryDate;

                if (request.MedicalCertificateDate != null)
                    fitMembership.MedicalCertificateDate = request.MedicalCertificateDate;

                // Save Medical Certificate File if provided
                if (request.MedicalCertificate != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.MedicalCertificate.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "medical_certificates", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.MedicalCertificate.CopyToAsync(stream);
                    }

                    fitMembership.MedicalCertificatePath = "/medical_certificates/" + fileName;
                }

                // Save the updated MembershipUser record
                _context.MembershipUsers.Update(fitMembership);
                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "FIT Membership updated successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = "Error updating FIT Membership", Data = ex.Message };
            }
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
