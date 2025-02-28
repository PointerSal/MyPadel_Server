using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model.Stripe
{
    public class PaymentRequest
    {
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
    }
    public class Refund
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; } // Stripe Payment ID
        public string OriginalEmail { get; set; } // Booking ka original email
        public string RefundRequestedBy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; } // Kitna paisa refund kia gaya
        public string RefundStatus { get; set; } // Pending, Processed, Failed
        public DateTime CreatedAt { get; set; } // Refund request date
    }
    public class RefundRequest
    {
        public string PaymentIntentId { get; set; }
        public string Email { get; set; } // Request karne wale ki email
    }


}
