using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public class FitMembershipTbl
    {
        [Key]
        public int Id { get; set; }
        public decimal FitMembershipFee { get; set; } // User-defined Fit Membership Fee
    }
}
