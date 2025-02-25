using AuthService.Bridge;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model.desktopmodel;

namespace AuthService.Services.desktopservice
{
    public class DesktopCourtSportsService : IDesktopCourtSportsService
    {
        private readonly ApplicationDbContext _context;

        public DesktopCourtSportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> AddCourtSportsAsync(CourtSportsRequest request)
        {
            try
            {
                var courtSports = new CourtSports
                {
                    SportsName = request.SportsName,
                    CourtFields = request.CourtFields
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


        // New method to fetch all CourtSports
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
