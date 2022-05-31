using Microsoft.Extensions.Configuration;
using Raven.Client.Documents.Session;
using ShopRite.Core.Configurations;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using StackExchange.Redis;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShopRite.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private const string USD = "usd";
        private readonly IDatabase _redis;
        private readonly StripeConfig _stripeConfig;
        private readonly IAsyncDocumentSession _db;

        public PaymentService(IConnectionMultiplexer redis, IAsyncDocumentSession db, IConfiguration configuration)
        {
            _redis = redis.GetDatabase();
            _stripeConfig = configuration.Get<StripeConfig>();
            _db = db;
        }
        public async Task CreateOrUpdatePaymentIntent(CustomerBasket basket, string postCompanyId, Domain.DistanceType distance)
        {
           
            StripeConfiguration.ApiKey = _stripeConfig.StripeSettings.SecretKey;
            var postCompany = await _db.LoadAsync<PostCompany>(postCompanyId);
            var shippingPrice = postCompany is null ? 0m : postCompany.DeliveryMethods[distance].DeliveryCost;
           
            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = basket.TotalPrice + (long)(shippingPrice * 100),
                    Currency = USD,
                    PaymentMethodTypes = new List<string>() { "card" },
                };
                paymentIntent = await paymentIntentService.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
                return;
            }
            var optionsUpdate = new PaymentIntentUpdateOptions
            {
                Amount = basket.TotalPrice + (long)(shippingPrice * 100),
            };
            await paymentIntentService.UpdateAsync(basket.PaymentIntentId, optionsUpdate);
        }

        public async Task<Domain.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var order = _db.Query<Domain.Order>().Where(x => x.PaymentIntentId == paymentIntentId).FirstOrDefault();
            if (order is null) return null;

            order.OrderStatus = OrderStatus.FromValue(OrderStatus.PaymentFailed);
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<Domain.Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var order = _db.Query<Domain.Order>().Where(x => x.PaymentIntentId == paymentIntentId).FirstOrDefault();
            if (order is null) return null;

            order.OrderStatus = OrderStatus.FromValue(OrderStatus.PaymentRecived);
            await _db.SaveChangesAsync();

            return order;
        }
       
    }
}
