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
        public async Task<IActionResult> RegisterMembershipUser([FromBody] MembershipUserRequest request) =>
            (await _membershipUserService.RegisterMembershipUser(request)).Code == "0000" ? Ok(await _membershipUserService.RegisterMembershipUser(request)) : StatusCode(500, await _membershipUserService.RegisterMembershipUser(request));


        [HttpPatch("fit")]
        public async Task<IActionResult> AlreadyFitMember([FromBody] FitMembershipRequest request) =>
            (await _membershipUserService.AlreadyFitMember(request)).Code == "0000" ? Ok(await _membershipUserService.AlreadyFitMember(request)) : StatusCode(500, await _membershipUserService.AlreadyFitMember(request));



        [HttpGet("carddetails/{email}")]
        public async Task<IActionResult> GetCardDetailsByEmail(string email) =>
      Ok(await _membershipUserService.GetCardDetailsByEmail(email));

        [HttpGet("expirydate/{email}")]
        public async Task<IActionResult> GetExpiryDateByEmail(string email) =>
            Ok(await _membershipUserService.GetExpiryDateByEmail(email));

    }
}
