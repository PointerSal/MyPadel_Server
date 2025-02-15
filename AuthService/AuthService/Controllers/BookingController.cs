using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [Route("api/booking")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Fetch available slots for a given date and field
        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] AvailableSlotsRequest request)
        {
            var result = await _bookingService.GetBookedSlotsAsync(request);
            return Ok(result);
        }

        // Reserve a booking
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveBooking([FromBody] ReserveBookingRequest request)
        {
            var result = await _bookingService.ReserveBookingAsync(request);
            if (result.Code == "0000")
            {
                return Ok(result);  // Success
            }

            return BadRequest(result);  // Failure
        }

        // Fetch user's bookings by email
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetUserBookings([FromQuery] GetUserBookingsRequest request)
        {
            var result = await _bookingService.GetUserBookingsAsync(request);
            return Ok(result);
        }

        // Cancel a booking by bookingId and email
        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest request)
        {
            var result = await _bookingService.CancelBookingAsync(request);
            if (result.Code == "0000")
            {
                return Ok(result);  // Success
            }

            return BadRequest(result);  // Failure
        }

        // Reschedule a booking
        [HttpPut("reschedule")]
        public async Task<IActionResult> RescheduleBooking([FromBody] RescheduleBookingRequest request)
        {
            var result = await _bookingService.RescheduleBookingAsync(request);
            if (result.Code == "0000")
            {
                return Ok(result);  // Success
            }

            return BadRequest(result);  // Failure
        }

        // Fetch booking history by email
        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory([FromQuery] BookingHistoryRequest request)
        {
            var result = await _bookingService.GetBookingHistoryAsync(request);
            return Ok(result);
        }

        // Fetch details of a specific booking
        [HttpGet("details/{bookingId}")]
        public async Task<IActionResult> GetBookingDetails([FromRoute] BookingDetailsRequest request)
        {
            var result = await _bookingService.GetBookingDetailsAsync(request);
            return Ok(result);
        }
    }
}
