using FluentEmail.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Raven.Client.Documents.Session;
using ShopRite.Core.Configurations;
using ShopRite.Domain;
using StackExchange.Redis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Orders
{
    public class CreateOrder
    {
        public class Command : IRequest<CreateOrderResponse>
        {
            public CreateOrderRequest CreateOrderRequest { get; set; }
        }
        public class Handler : IRequestHandler<Command, CreateOrderResponse>
        {
            private readonly IDatabase _redis;
            private readonly IAsyncDocumentSession _db;
            private readonly IFluentEmail _fluentEmail;
            private readonly GlobalConfiguration _globalConfig;

            public Handler(IConnectionMultiplexer redis,
                           IAsyncDocumentSession db,
                           IConfiguration config,
                           IFluentEmail fluentEmail)
            {
                _redis = redis.GetDatabase();
                _db = db;
                _fluentEmail = fluentEmail;
                _globalConfig = config.Get<GlobalConfiguration>();
            }
            public async Task<CreateOrderResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = await _redis.StringGetAsync(request.CreateOrderRequest.BasketId);
                var response = new CreateOrderResponse();
                var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
                foreach (var orderItem in basket?.Items)
                {
                    var product = await _db.LoadAsync<Product>(orderItem.Product.Id);
                    var stocksDict = product.Stocks.ToDictionary(key => key.Size, value => value.Quantity);
                    var isOutOfStock = basket.Items.Sum(x => x.Sizes.Sum(x => x.Quantity)) > stocksDict.Sum(x => x.Value);
                    foreach (var requestedSize in orderItem.Sizes)
                    {
                        if (stocksDict.ContainsKey(requestedSize.Size))
                        {
                            var currentQuantity = product.Stocks
                                                 .FirstOrDefault(x => x.Size == requestedSize.Size);
                            var isSuccessful = TrySubtractFromStock(product,
                                                                    requestedSize,
                                                                    currentQuantity,
                                                                    response,
                                                                    isOutOfStock);

                        }
                    }
                }
                await _db.SaveChangesAsync();
                if (response.SuccessfulOrders[false].Any())
                {
                    var path = Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "ShopRite.Platform", "Views"));
                    _fluentEmail.To(_globalConfig.Emails.RetailMail)
                                .UsingTemplateFromFile(@$"{path}\OutOfStock.cshtml", response.SuccessfulOrders[false]);
                    await _fluentEmail.SendAsync();
                }

                return response;
            }

            private bool TrySubtractFromStock(Product product, Stock requestedSize, Stock currentQuantity, CreateOrderResponse response, bool isOutOfStock)
            {
                if (currentQuantity.Quantity >= requestedSize.Quantity && !isOutOfStock)
                {
                    currentQuantity.Quantity -= requestedSize.Quantity;
                    response.SuccessfulOrders[true]
                        .Add(new OrderDTO { Name = product.Name, Size = currentQuantity.Size });
                    return true;
                }
                if (requestedSize.Quantity > currentQuantity.Quantity && isOutOfStock)
                {
                    response.SuccessfulOrders[false]
                     .Add(new OrderDTO { Name = product.Name, Size = currentQuantity.Size });
                    product.IsOutOfStock = true;
                    response.ProductsOutOfStack.Add($"Product {product.Name} is out of stack for size {requestedSize.Size}.");
                }

                return false;
            }
        }
        public class CreateOrderRequest
        {
            public string BuyerEmail { get; set; }
            public string BasketId { get; set; }
        }

        public class CreateOrderResponse
        {
            public List<string> ProductsOutOfStack { get; set; } = new();
            public Dictionary<bool, List<OrderDTO>> SuccessfulOrders { get; set; }
                      = new()
                      {
                          { true, new List<OrderDTO>() },
                          { false, new List<OrderDTO>() },
                      };
        }
        public class OrderDTO
        {
            public string Name { get; set; }
            public string Size { get; set; }
        }
    }
}
