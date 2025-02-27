using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model
{
    public class FitMembershipTbl
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FitMembershipFee { get; set; } // User-defined Fit Membership Fee
    }
}
