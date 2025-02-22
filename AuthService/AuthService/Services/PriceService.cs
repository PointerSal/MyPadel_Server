using System.Collections.Generic;
using System.Threading.Tasks;
using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class PriceService : IPriceService
    {
        private readonly ApplicationDbContext _context;

        public PriceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Status> GetPricesAsync()
        {
            var prices = await _context.PriceTbls.ToListAsync();
            if (prices == null || prices.Count == 0)
                return new Status { Code = "1001", Message = "No prices found.", Data = null };

            return new Status { Code = "0000", Message = "Prices fetched successfully.", Data = prices };
        }

        public async Task<Status> AddPriceAsync(PriceTbl price)
        {
            if (price.Duration <= 0 || price.Price <= 0)
                return new Status { Code = "1002", Message = "Invalid duration or price.", Data = null };

            _context.PriceTbls.Add(price);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Price added successfully.", Data = price };
        }

        public async Task<Status> GetFitMembershipFeeAsync()
        {
            var fitMembership = await _context.FitMembershipTbls.FirstOrDefaultAsync();
            if (fitMembership == null)
                return new Status { Code = "1003", Message = "No fit membership fee found.", Data = null };

            return new Status { Code = "0000", Message = "Fit membership fee fetched successfully.", Data = fitMembership };
        }

        public async Task<Status> AddFitMembershipFeeAsync(FitMembershipTbl fitMembership)
        {
            if (fitMembership.FitMembershipFee <= 0)
                return new Status { Code = "1004", Message = "Invalid fit membership fee.", Data = null };

            _context.FitMembershipTbls.Add(fitMembership);
            await _context.SaveChangesAsync();

            return new Status { Code = "0000", Message = "Fit membership fee added successfully.", Data = fitMembership };
        }
    }
}
