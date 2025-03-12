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

        // Get all users
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers() =>
            Ok(await _desktopClientService.GetAllUsersAsync());


        // Get customer booking history
        [HttpGet("booking-history")]
        public async Task<IActionResult> GetCustomerBookingHistory([FromQuery] string email) =>
            Ok(await _desktopClientService.GetCustomerBookingHistoryAsync(email));

        // Search desktop client by email
        [HttpGet("search")]
        public async Task<IActionResult> SearchDesktopClient([FromQuery] DesktopClientSearchRequest request) =>
            Ok(await _desktopClientService.SearchDesktopClientByEmailAsync(request.Email));

        // Update desktop client information
        [HttpPatch("update")]
        public async Task<IActionResult> UpdateDesktopClient([FromBody] DesktopClientUpdateRequest request) =>
            Ok(await _desktopClientService.UpdateDesktopClientInformationAsync(request));

        [HttpGet("customer-statistics")]
        public async Task<IActionResult> GetCustomerStatistics([FromQuery] string email) => Ok(await _desktopClientService.GetCustomerStatisticsAsync(email));



        // Delete user by email
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email) =>
            Ok(await _desktopClientService.DeleteUserAsync(email));



    }
}
