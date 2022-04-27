using Ardalis.GuardClauses;
using MediatR;
using ShopRite.Domain;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Basket
{
    public class GetBasket
    {
        public class Query : IRequest<BasketResponse>
        {
            public string Id { get; set; }
        }
        public class QueryHandler : IRequestHandler<Query, BasketResponse>
        {
            private readonly IDatabase _db;
            public QueryHandler(IConnectionMultiplexer db)
            {
                _db = db.GetDatabase();
            }

            public async Task<BasketResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = await _db.StringGetAsync(request.Id);
                var customerBasket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
                Guard.Against.Null(customerBasket, nameof(customerBasket));
                return new BasketResponse { Id = request.Id, Items = customerBasket.Items };
            }
        }
        public class BasketResponse
        {
            public string Id { get; set; }
            public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        }
    }
}
