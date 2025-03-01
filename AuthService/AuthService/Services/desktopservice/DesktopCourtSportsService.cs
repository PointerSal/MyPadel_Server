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

        public async Task<Status> DeleteFieldAsync(CourtSportsRequestDelete requestDelete)
        {
            try
            {
                var courtSport = await _context.CourtSports
             .FirstOrDefaultAsync(c => c.Id == requestDelete.Id);  

                if (courtSport == null)
                    return new Status { Code = "1003", Message = "Field not found", Data = null };

                _context.CourtSports.Remove(courtSport);
                await _context.SaveChangesAsync();

                return new Status { Code = "0000", Message = "Field deleted successfully", Data = null };
            }
            catch (Exception ex)
            {
                return new Status { Code = "1004", Message = "Error deleting field", Data = ex.Message };
            }
        }

        public async Task<Status> UpdateCourtSportsAsync(CourtSportsRequestEdit requestEdit)
        {
            try
            {
                // Verify that the record with the provided Id exists
                var courtSport = await _context.CourtSports
                    .FirstOrDefaultAsync(c => c.Id == requestEdit.Id);  // Find by Id

                if (courtSport == null)
                    return new Status { Code = "1003", Message = "Field not found", Data = null };

                // Proceed with updating other fields if they are provided
                if (!string.IsNullOrEmpty(requestEdit.SportsName)) courtSport.SportsName = requestEdit.SportsName;
                if (!string.IsNullOrEmpty(requestEdit.FieldName)) courtSport.FieldName = requestEdit.FieldName;
                if (!string.IsNullOrEmpty(requestEdit.FieldType)) courtSport.FieldType = requestEdit.FieldType;
                if (!string.IsNullOrEmpty(requestEdit.TerrainType)) courtSport.TerrainType = requestEdit.TerrainType;
                if (requestEdit.FieldCapacity.HasValue) courtSport.FieldCapacity = requestEdit.FieldCapacity.Value;
                if (requestEdit.Slot1Duration.HasValue) courtSport.Slot1Duration = requestEdit.Slot1Duration.Value;
                if (requestEdit.Slot1Price.HasValue) courtSport.Slot1Price = requestEdit.Slot1Price.Value;
                if (requestEdit.Slot2Duration.HasValue) courtSport.Slot2Duration = requestEdit.Slot2Duration.Value;
                if (requestEdit.Slot2Price.HasValue) courtSport.Slot2Price = requestEdit.Slot2Price.Value;
                if (requestEdit.Slot3Duration.HasValue) courtSport.Slot3Duration = requestEdit.Slot3Duration.Value;
                if (requestEdit.Slot3Price.HasValue) courtSport.Slot3Price = requestEdit.Slot3Price.Value;
                if (requestEdit.CanBeBooked.HasValue) courtSport.CanBeBooked = requestEdit.CanBeBooked.Value;
                if (!string.IsNullOrEmpty(requestEdit.OpeningHours)) courtSport.OpeningHours = requestEdit.OpeningHours;

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return new Status
                {
                    Code = "0000",
                    Message = "Court sports updated successfully",
                    Data = courtSport
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    Code = "1006",
                    Message = "Error updating court sports",
                    Data = ex.Message
                };
            }
        }



    }


}