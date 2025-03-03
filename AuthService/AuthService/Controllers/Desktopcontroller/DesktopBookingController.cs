using AuthService.Interfaces.desktopinterface;
using AuthService.Model;
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
        public async Task<IActionResult> GetAllBookings([FromQuery] DateTime date) =>
            Ok(await _desktopBookingService.GetBookingsByDateAsync(date));


        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromQuery] int bookingId) =>
           Ok(await _desktopBookingService.CancelBookingAsync(bookingId));

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveBooking([FromBody] ReserveBookingRequest request) =>
            Ok(await _desktopBookingService.ReserveBookingAsync(request));




    }
}
