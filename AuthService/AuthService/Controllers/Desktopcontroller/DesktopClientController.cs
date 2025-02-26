using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.Interfaces.DesktopInterface;
using AuthService.Models.DesktopModel;

namespace AuthService.Controllers.DesktopController
{
    [Route("api/desktop-client")]
    [ApiController]
    public class DesktopClientController : ControllerBase
    {
        private readonly IDesktopClientService _desktopClientService;

        public DesktopClientController(IDesktopClientService desktopClientService) => _desktopClientService = desktopClientService;

        // Search desktop client by email
        [HttpGet("search")]
        public async Task<IActionResult> SearchDesktopClient([FromQuery] DesktopClientSearchRequest request) =>
            Ok(await _desktopClientService.SearchDesktopClientByEmailAsync(request.Email));

        // Update desktop client information
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDesktopClient([FromBody] DesktopClientUpdateRequest request) =>
            Ok(await _desktopClientService.UpdateDesktopClientInformationAsync(request));
    }
}
