﻿using AuthService.Bridge;

namespace AuthService.Interfaces.desktopinterface
{
    public interface IDesktopBookingService
    {
        Task<Status> GetAllBookingsAsync();
    }
}
