using AuthService.Bridge;
using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IUserService
    {
        Task<Status> GetUserProfileAsync(string email);
        Task<Status> UpdateUserProfileAsync(UpdateProfileRequest request);
    }
}
