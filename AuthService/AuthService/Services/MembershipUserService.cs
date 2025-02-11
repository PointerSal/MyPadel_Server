using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;

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
                var fitMembership = new MembershipUser
                {
                    CardNumber = request.MembershipNumber,
                    ExpiryDate = request.ExpiryDate,
                    MedicalCertificateDate = request.MedicalCertificateDate,
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

                    fitMembership.MedicalCertificatePath = "/medical_certificates/" + fileName;
                }

                _context.MembershipUsers.Add(fitMembership);
                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "FIT Membership registered successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1001", Message = "Error registering FIT Membership", Data = ex.Message };
            }
        }


    }

}
