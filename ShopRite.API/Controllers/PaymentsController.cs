using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopRite.Core.Configurations;
using ShopRite.Core.Interfaces;
using ShopRite.Core.Responses;
using ShopRite.Domain;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly StripeConfig _stripeConfig;

        public PaymentsController(ILogger<PaymentsController> logger, IPaymentService paymentService, IConfiguration configuration)
        {
            _logger = logger;
            _paymentService = paymentService;
            _stripeConfig = configuration.Get<StripeConfig>();
        }
       
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _stripeConfig.StripeSettings.WebhookSecret);

            PaymentIntent intent;
            Domain.Order order;
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;

                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Payment Failed: ", order.Id);
                    break;

            }

            return new EmptyResult();



        }

    }
}
