using AuthService.Bridge;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IDesktopBookingService
    {
        Task<Status> GetBookingsByDateAsync(DateTime date);
        Task<Status> CancelBookingAsync(int bookingId);


    }
}
