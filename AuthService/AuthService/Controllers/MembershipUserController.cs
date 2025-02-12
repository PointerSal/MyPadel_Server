using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/membershipuser")]
    [ApiController]
    [Authorize]
    public class MembershipUserController : ControllerBase
    {
        private readonly IMembershipUserService _membershipUserService;

        public MembershipUserController(IMembershipUserService membershipUserService)
        {
            _membershipUserService = membershipUserService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterMembershipUser([FromForm] MembershipUserRequest request)
        {
            var result = await _membershipUserService.RegisterMembershipUser(request);
            if (result.Code == "0000")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("register/fit")]
        public async Task<IActionResult> AlreadyFitMember([FromForm] FitMembershipRequest request)
        {
            var result = await _membershipUserService.AlreadyFitMember(request);  // Using the updated service method
            if (result.Code == "0000")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }

}
