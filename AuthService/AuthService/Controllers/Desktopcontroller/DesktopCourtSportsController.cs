using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model.desktopmodel;
using Microsoft.AspNetCore.Http;

namespace AuthService.Controllers.Desktopcontroller
{
    [Route("api/courtsports")]
    [ApiController]
    public class DesktopCourtSportsController : ControllerBase
    {
        private readonly IDesktopCourtSportsService _desktopCourtSportsService;

        public DesktopCourtSportsController(IDesktopCourtSportsService desktopCourtSportsService) => _desktopCourtSportsService = desktopCourtSportsService;

        [HttpPost("add")]
        public async Task<IActionResult> AddCourtSports([FromBody] CourtSportsRequest request) => Ok(await _desktopCourtSportsService.AddCourtSportsAsync(request));

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourtSports() => Ok(await _desktopCourtSportsService.GetAllCourtSportsAsync());

        [HttpDelete("deletefield")]
        public async Task<IActionResult> DeleteField([FromBody] CourtSportsRequestDelete requestDelete) =>
            Ok(await _desktopCourtSportsService.DeleteFieldAsync(requestDelete));

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCourtSports([FromBody] CourtSportsRequestEdit requestEdit) =>
            Ok(await _desktopCourtSportsService.UpdateCourtSportsAsync(requestEdit));


    }
}