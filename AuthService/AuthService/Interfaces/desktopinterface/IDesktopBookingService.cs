using AuthService.Bridge;
using AuthService.Model;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IDesktopBookingService
    {
        Task<Status> GetBookingsByDateAsync(DateTime date);
        Task<Status> CancelBookingAsync(int bookingId);
        Task<Status> ReserveBookingAsync(ReserveBookingRequest request); 


    }
}
