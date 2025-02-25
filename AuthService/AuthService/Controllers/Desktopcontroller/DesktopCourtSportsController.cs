using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model.desktopmodel;

namespace AuthService.Controllers.Desktopcontroller
{
    [Route("api/courtsports")]
    [ApiController]
    public class DesktopCourtSportsController : ControllerBase
    {
        private readonly IDesktopCourtSportsService _desktopCourtSportsService;

        public DesktopCourtSportsController(IDesktopCourtSportsService desktopCourtSportsService)
        {
            _desktopCourtSportsService = desktopCourtSportsService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCourtSports([FromBody] CourtSportsRequest request) => Ok(await _desktopCourtSportsService.AddCourtSportsAsync(request));

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourtSports() => Ok(await _desktopCourtSportsService.GetAllCourtSportsAsync());




    }
}
