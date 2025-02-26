using AuthService.Bridge;
 using AuthService.Models.DesktopModel;
 
namespace AuthService.Interfaces.DesktopInterface
{
    public interface IDesktopClientService
    {
        Task<Status> SearchDesktopClientByEmailAsync(string email);
        Task<Status> UpdateDesktopClientInformationAsync(DesktopClientUpdateRequest request);
    }
}
