using System.Threading.Tasks;
using AuthService.Model.desktopmodel;
using AuthService.Interfaces.desktopinterface;
using AuthService.Bridge;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.desktopservice
{
    public class DesktopCourtSportsService : IDesktopCourtSportsService
    {
        private readonly ApplicationDbContext _context;

        public DesktopCourtSportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add Court Sports Reservation
        public async Task<Status> AddCourtSportsAsync(CourtSportsRequest request)
        {
            try
            {
                var courtSports = new CourtSports
                {
                    SportsName = request.SportsName,
                    FieldName = request.FieldName,
                    FieldType = request.FieldType,
                    TerrainType = request.TerrainType,
                    FieldCapacity = request.FieldCapacity,
                    Slot1Duration = request.Slot1Duration,
                    Slot1Price = request.Slot1Price,
                    Slot2Duration = request.Slot2Duration,
                    Slot2Price = request.Slot2Price,
                    Slot3Duration = request.Slot3Duration,
                    Slot3Price = request.Slot3Price,
                    CanBeBooked = request.CanBeBooked,
                    OpeningHours = request.OpeningHours
                };

                _context.CourtSports.Add(courtSports);
                await _context.SaveChangesAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Court sports added successfully",
                    Data = courtSports
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1001",
                    Message = "Error adding court sports",
                    Data = ex.Message
                };
            }
        }

        // Get All Court Sports Reservations
        public async Task<Status> GetAllCourtSportsAsync()
        {
            try
            {
                var allCourtSports = await _context.CourtSports.ToListAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Court sports fetched successfully",
                    Data = allCourtSports
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1002",
                    Message = "Error fetching court sports data",
                    Data = ex.Message
                };
            }
        }
    }
}
