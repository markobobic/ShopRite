using Coravel.Mailer.Mail.Interfaces;
using MediatR;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using StackExchange.Redis;
using System.Collections.Generic;
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
            private readonly IMailer _mailer;

            public Handler(IConnectionMultiplexer redis, IAsyncDocumentSession db, IMailer mailer)
            {
                _redis = redis.GetDatabase();
                _db = db;
                _mailer = mailer;
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
                    foreach (var requestedSize in orderItem.Sizes)
                    {
                        if (stocksDict.ContainsKey(requestedSize.Size))
                        {
                            var currentQuantity = product.Stocks
                                                 .FirstOrDefault(x => x.Size == requestedSize.Size);
                            var isSuccessful = TrySubtractFromStock(product, requestedSize, currentQuantity, response);
                            if (isSuccessful is not true) break;
                        }
                    }

                }
                if (!response.SuccessfulOrders[false].Any())
                    await _db.SaveChangesAsync();

                return response;
            }

            private bool TrySubtractFromStock(Product product, Stock requestedSize, Stock currentQuantity, CreateOrderResponse response)
            {
                if (currentQuantity.Quantity >= requestedSize.Quantity)
                {
                    currentQuantity.Quantity -= requestedSize.Quantity;
                    response.SuccessfulOrders[true]
                        .Add(new OrderDTO { Id = product.Id, Size = currentQuantity.Size });
                    return true;
                }
                response.SuccessfulOrders[false]
                    .Add(new OrderDTO { Id = product.Id, Size = currentQuantity.Size });

                response.ProductsOutOfStack.Add($"Product {product.Name} is out of stack for size {requestedSize.Size}.");
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
            public string Id { get; set; }
            public string Size { get; set; }
        }
    }
}
