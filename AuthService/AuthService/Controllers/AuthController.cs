using AuthService.Interfaces;
using AuthService.Models;
using AuthService.Bridge;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterUser(request);
            return Ok(result);
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginUser(request);
            if (result.Code == "0000")
            {
                return Ok(result);  
            }
            return Unauthorized(result);  
        }

        /// <summary>
        /// Verifies email with OTP
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await _authService.VerifyEmail(request);
            return Ok(result);
        }

        [HttpPost("resendEmail-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOTPRequest request)
        {
            var result = await _authService.ResendEmailOTP(request.Email);

            if (result.Code == "0000")
            {
                return Ok(new { Message = "OTP resent successfully" });
            }
            return BadRequest(result);
        }
        
        [HttpPost("add-phone-number")]
        public async Task<IActionResult> AddPhoneNumber([FromBody] AddPhoneNumberRequest request)
        {
            var result = await _authService.AddPhoneNumber(request.Email, request.Cell);
            return Ok(result);
        }
        [HttpPost("resendPhone-otp")]
        public async Task<IActionResult> ResendPhoneOTP([FromBody] ResendOTPRequest request)
        {
            var result = await _authService.ResendPhoneOTP(request.Email);

            if (result.Code == "0000")
            {
                return Ok(new { Message = "Phone OTP resent successfully" });
            }
            return BadRequest(result);
        }
        /// <summary>
        /// Verifies phone with OTP
        /// </summary>
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequest request)
        {
            var result = await _authService.VerifyPhone(request);
            return Ok(result);
        }

        /// <summary>
        /// Resets password
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPassword(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a user account
        /// </summary>
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            var result = await _authService.DeleteAccount(request);
            return Ok(result);
        }
    }
}
