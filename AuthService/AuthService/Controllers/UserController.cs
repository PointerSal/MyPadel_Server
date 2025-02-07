using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] string email)
        {
            
            string userEmail = string.IsNullOrEmpty(email) ? User.Identity.Name : email;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(new Status { Code = "1003", Message = "Email is required", Data = null });
            }

            var result = await _userService.GetUserProfileAsync(userEmail);
            if (result.Code == "0000")
            {
                return Ok(result);  
            }
            return BadRequest(result);  
        }

        
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await _userService.UpdateUserProfileAsync(request);
            if (result.Code == "0000")
            {
                return Ok(result);  
            }
            return BadRequest(result);  
        }
    }
}
