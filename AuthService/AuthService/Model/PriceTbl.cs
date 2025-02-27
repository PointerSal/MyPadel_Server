using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model
{
    public class PriceTbl
    {
        [Key]
        public int Id { get; set; }
        public int Duration { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal Price { get; set; } 
    }
}
