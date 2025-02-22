using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public class PriceTbl
    {
        [Key]
        public int Id { get; set; }
        public int Duration { get; set; }   
        public decimal Price { get; set; } 
    }
}
