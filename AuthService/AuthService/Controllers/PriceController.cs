using System.Threading.Tasks;
using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/price")]
    [ApiController]
    [Authorize]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PriceController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPrices() => Ok(await _priceService.GetPricesAsync());

        [HttpPost]
        public async Task<IActionResult> AddPrice([FromBody] PriceTbl price) => Ok(await _priceService.AddPriceAsync(price));

        [HttpGet("fitmembership")]
        public async Task<IActionResult> GetFitMembershipFee() => Ok(await _priceService.GetFitMembershipFeeAsync());

        [HttpPost("fitmembership")]
        public async Task<IActionResult> AddFitMembershipFee([FromBody] FitMembershipTbl fitMembership)
            => Ok(await _priceService.AddFitMembershipFeeAsync(fitMembership));
    }
}
