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

            public Handler(IConnectionMultiplexer redis, IAsyncDocumentSession db)
            {
                _redis = redis.GetDatabase();
                _db = db;
            }
            // neko je narucio Nike proizvod Quantity 3 patike stocks size 32 i size 34
            public async Task<CreateOrderResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var data = await _redis.StringGetAsync(request.CreateOrderRequest.BasketId);
                var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
                foreach (var orderItem in basket.Items)
                {
                    var product = await _db.LoadAsync<Product>(orderItem.Product.Id);
                    var stocksDict = product.Stocks.ToDictionary(key => key.Size, value => value.Quantity);
                    foreach (var requestedSize in orderItem.Sizes)
                    {
                        if (stocksDict.ContainsKey(requestedSize.Size))
                        {
                           var currentQuantity = product.Stocks.FirstOrDefault(x => x.Size == requestedSize.Size);
                            currentQuantity.Quantity -= requestedSize.Quantity;
                        }
                    }
                  
                }
                await _db.SaveChangesAsync(cancellationToken);
                return new CreateOrderResponse();
            }
        }
        public class CreateOrderRequest
        {
            public string BuyerEmail { get; set; }
            public string BasketId { get; set; }
        }

        public class CreateOrderResponse
        {
        }
    }
}
