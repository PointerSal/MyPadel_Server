using System.Threading.Tasks;
using AuthService.Bridge;
using AuthService.Model.desktopmodel;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IDesktopCourtSportsService
    {
        Task<Status> AddCourtSportsAsync(CourtSportsRequest request);
        Task<Status> GetAllCourtSportsAsync();
        Task<Status> DeleteFieldAsync(CourtSportsRequestDelete requestDelete); 

    }
}
