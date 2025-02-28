using AuthService.Bridge;
using AuthService.Model;
using AuthService.Model.Stripe;
using System.Threading.Tasks;

namespace AuthService.Interfaces
{
    public interface IStripeService
    {
        Task<Status> CreateCheckoutSession(PaymentRequest request);
        Task<bool> CompletePayment(string sessionId, string paymentStatus);
        Task ProcessWebhook(string json, string stripeSignature);
        Task<Status> ProcessRefund(string paymentIntentId, string refundRequestedBy);

    }
}
