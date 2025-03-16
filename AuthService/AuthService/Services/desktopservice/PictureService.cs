using AuthService.Bridge;
using AuthService.Interfaces.desktopinterface;
using AuthService.Interfaces.DesktopInterface;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.DesktopService
{
    public class PictureService : IPictureService
    {
        private readonly ApplicationDbContext _context;

        public PictureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> GetMedicalCertificateAsync(string email)
        {
            // Direct query to fetch only the necessary fields (MedicalCertificatePath) from MembershipUsers using email
            var medicalCertificatePath = await _context.MembershipUsers
                .Where(m => m.Email == email)
                .Select(m => m.MedicalCertificatePath)
                .FirstOrDefaultAsync();

            // Check if the MedicalCertificatePath exists
            if (string.IsNullOrEmpty(medicalCertificatePath))
            {
                return new Status { Code = "1002", Message = "Medical Certificate not found", Data = null };
            }

            // Return the medical certificate path successfully
            return new Status
            {
                Code = "0000",
                Message = "Medical Certificate fetched successfully",
                Data = new { MedicalCertificatePath = medicalCertificatePath }
            };
        }
    }
}
