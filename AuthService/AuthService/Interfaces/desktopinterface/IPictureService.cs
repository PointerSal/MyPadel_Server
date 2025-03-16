using AuthService.Bridge;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IPictureService
    {
        Task<Status> GetMedicalCertificateAsync(string email);
    }

}
