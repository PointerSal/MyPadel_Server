using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [HttpPatch("update/fit")]
        public async Task<IActionResult> UpdateFitMember([FromForm] FitMembershipRequest request)
        {
            var result = await _membershipUserService.AlreadyFitMember(request);
            if (result.Code == "0000")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }



        [HttpGet("carddetails/{email}")]
        public async Task<IActionResult> GetCardDetailsByEmail(string email)
        {
            var result = await _membershipUserService.GetCardDetailsByEmail(email);
            if (result.Code == "0000")
            {
                return Ok(result);
            }
            return NotFound(result);
        }


        [HttpGet("expirydate/{email}")]
        public async Task<IActionResult> GetExpiryDateByEmail(string email)
        {
            var result = await _membershipUserService.GetExpiryDateByEmail(email);
            if (result.Code == "0000")
            {
                return Ok(result);
            }
            return NotFound(result);
        }

    }
}
