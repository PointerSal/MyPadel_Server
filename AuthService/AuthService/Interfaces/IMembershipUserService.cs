using AuthService.Bridge;
using AuthService.Model;

namespace AuthService.Interfaces
{
    public interface IMembershipUserService
    {
        Task<Status> RegisterMembershipUser(MembershipUserRequest request);

        Task<Status> AlreadyFitMember(FitMembershipRequest request);
    }

}
