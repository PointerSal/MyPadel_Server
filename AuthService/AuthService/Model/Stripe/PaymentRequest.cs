namespace AuthService.Model.Stripe
{
    public class PaymentRequest
    {
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
    }
}
