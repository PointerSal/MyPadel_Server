using AuthService.Bridge;
 using AuthService.Models.DesktopModel;
 
namespace AuthService.Interfaces.DesktopInterface
{
    public interface IDesktopClientService
    {
        Task<Status> SearchDesktopClientByEmailAsync(string email);
        Task<Status> UpdateDesktopClientInformationAsync(DesktopClientUpdateRequest request);
        Task<Status> GetAllUsersAsync();
        Task<Status> GetCustomerBookingHistoryAsync(string email);
        Task<Status> GetCustomerStatisticsAsync(string email);

        Task<Status> RenewMembershipAsync(string email);
        Task<Status> DeleteUserAsync(string email);


    }
}
