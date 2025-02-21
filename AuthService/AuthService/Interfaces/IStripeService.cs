using AuthService.Model;
using AuthService.Model.Stripe;
using System.Threading.Tasks;

namespace AuthService.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSession(PaymentRequest request);
        Task<bool> CompletePayment(string sessionId, string paymentStatus);
        Task ProcessWebhook(string json, string stripeSignature);
    }
}
