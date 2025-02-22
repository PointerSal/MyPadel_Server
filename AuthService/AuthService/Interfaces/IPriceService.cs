using System.Threading.Tasks;
using AuthService.Bridge;
using AuthService.Model;

namespace AuthService.Interfaces
{
    public interface IPriceService
    {
        Task<Status> GetPricesAsync();
        Task<Status> AddPriceAsync(PriceTbl price);
        Task<Status> GetFitMembershipFeeAsync();
        Task<Status> AddFitMembershipFeeAsync(FitMembershipTbl fitMembership);
    }
}
