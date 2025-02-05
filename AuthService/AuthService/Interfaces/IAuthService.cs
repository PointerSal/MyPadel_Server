using AuthService.Bridge;
using AuthService.Models;
using System.Threading.Tasks;

namespace AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<Status> RegisterUser(RegisterRequest request);
        Task<Status> LoginUser(LoginRequest request);
        Task<Status> VerifyEmail(VerifyEmailRequest request);
        Task<Status> VerifyPhone(VerifyPhoneRequest request);
        Task<Status> AddPhoneNumber(string email, string phoneNumber);  
        Task<Status> ResendPhoneOTP(string email);
        Task<Status> ResetPassword(ResetPasswordRequest request);
        Task<Status> DeleteAccount(DeleteAccountRequest request);
        Task<Status> ResendEmailOTP(string email);  
    }
}
