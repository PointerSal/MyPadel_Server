using AuthService.Bridge;
using AuthService.Interfaces;
using AuthService.Model.Stripe;
using Microsoft.AspNetCore.Mvc;

[Route("api/stripe")]
[ApiController]
public class StripeController : ControllerBase
{
    private readonly IStripeService _stripeService;

    public StripeController(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    // Step 1: Create Checkout Session
    [HttpPost("checkout-session")]
    public async Task<IActionResult> CheckoutSession([FromBody] PaymentRequest request) =>
        Ok(await _stripeService.CreateCheckoutSession(request));

    // Step 2: Handle Payment Success
    [HttpGet("success")]
    public async Task<IActionResult> PaymentSuccess([FromQuery] string session_id)
    {
        var result = await _stripeService.CompletePayment(session_id, "Paid");
        return result
            ? Ok(new Status { Code = "0000", Message = "Payment Successful" })
            : BadRequest(new Status { Code = "1001", Message = "Payment Failed" });
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundRequest request)
    {
        var result = await _stripeService.ProcessRefund(request.PaymentIntentId, request.Email);
        return result.Code == "0000" ? Ok(result) : BadRequest(result);
    }



    // Step 3: Handle Payment Cancel
    [HttpGet("cancel")]
    public IActionResult PaymentCancel() => Ok(new Status { Code = "1001", Message = "Payment Canceled" });

    // Step 4: Stripe Webhook to process payment events (success/failure)
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];
        await _stripeService.ProcessWebhook(json, stripeSignature);
        return Ok();
    }
}
