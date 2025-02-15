using System.Threading.Tasks;
using AuthService.Bridge;
using AuthService.Model;

namespace AuthService.Interfaces
{
    public interface IBookingService
    {
        Task<Status> GetBookedSlotsAsync(AvailableSlotsRequest request);

        Task<Status> ReserveBookingAsync(ReserveBookingRequest request);     // Reserve a booking
        Task<Status> GetUserBookingsAsync(GetUserBookingsRequest request);   // Fetch a user's bookings by email
        Task<Status> CancelBookingAsync(CancelBookingRequest request);       // Cancel a booking
        Task<Status> RescheduleBookingAsync(RescheduleBookingRequest request);  // Reschedule a booking
        Task<Status> GetBookingHistoryAsync(BookingHistoryRequest request);  // Fetch a user's booking history
        Task<Status> GetBookingDetailsAsync(BookingDetailsRequest request); // Fetch details of a specific booking
    }
}
