using AuthService.Interfaces.desktopinterface;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Desktopcontroller
{
    [Route("api/desktop-booking")]
    [ApiController]
    public class DesktopBookingController : ControllerBase
    {
        private readonly IDesktopBookingService _desktopBookingService;

        public DesktopBookingController(IDesktopBookingService desktopBookingService)
        {
            _desktopBookingService = desktopBookingService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings() => Ok(await _desktopBookingService.GetAllBookingsAsync());
    }
}
