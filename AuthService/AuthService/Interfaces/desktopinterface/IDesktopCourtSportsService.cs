using System.Threading.Tasks;
using AuthService.Bridge;
using AuthService.Model.desktopmodel;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IDesktopCourtSportsService
    {
        Task<Status> AddCourtSportsAsync(CourtSportsRequest request);
        Task<Status> GetAllCourtSportsAsync();  // New method to get all CourtSports
    }

}
