using AuthService.Bridge;
using AuthService.Model;
using System.Threading.Tasks;

namespace AuthService.Interfaces
{
    public interface IMembershipUserService
    {
        Task<Status> RegisterMembershipUser(MembershipUserRequest request);
        Task<Status> AlreadyFitMember(FitMembershipRequest request);
        Task<Status> GetCardDetailsByEmail(string email);
        Task<Status> GetExpiryDateByEmail(string email); 

    }
}
