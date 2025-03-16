using AuthService.Interfaces.desktopinterface;
using AuthService.Interfaces.DesktopInterface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthService.Controllers.DesktopController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        // Updated to use email as a request parameter
        [HttpGet("medical-certificate")]
        public async Task<IActionResult> GetMedicalCertificate([FromQuery] string email) =>
            Ok(await _pictureService.GetMedicalCertificateAsync(email));
    }
}
