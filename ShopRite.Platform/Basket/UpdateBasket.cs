using Ardalis.GuardClauses;
using MediatR;
using Raven.Client.Documents;
using ShopRite.Core.Extensions;
using ShopRite.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Basket
{
    public class UpdateBasket
    {
        public class Command : IRequest<BasketUpdateResponse>
        {

            public BasketUpdateRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Command, BasketUpdateResponse>
        {
            private const int ThirtyDays = 30;
            private readonly IDocumentStore _ravenDb;
            private readonly IDatabase _db;
            public Handler(IConnectionMultiplexer db, IDocumentStore ravenDb)
            {

                _db = db.GetDatabase();
                _ravenDb = ravenDb;
            }
            public async Task<BasketUpdateResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var session = _ravenDb.OpenAsyncSession();
                var customerBasket = new CustomerBasket();
                foreach (var productDto in request.Request.Products)
                {
                    var product = await session.LoadAsync<Product>(productDto.ProductId);
                    customerBasket.Items
                        .Add(new BasketItem() 
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            ProductBrand = product.ProductBrand,
                            ProductType = product.ProductType,
                            Sizes = productDto.Sizes 
                        });
                }
                
                var isCreated = await _db.
                    StringSetAsync(customerBasket.Id,
                    JsonSerializer.Serialize(customerBasket),
                    TimeSpan.FromDays(ThirtyDays));

               return new BasketUpdateResponse { CustomerBasket = customerBasket, IsBasketUpdated = isCreated };
            }
        }
        public class BasketUpdateRequest
        {
            public List<ProductBasketDTO> Products { get; set; }
            public class ProductBasketDTO
            {
                public string ProductId { get; set; }
                public List<Stock> Sizes { get; set; } = new List<Stock>();
            }

        }
        
        public class BasketUpdateResponse
        {
            public CustomerBasket CustomerBasket { get; set; }
            public bool IsBasketUpdated { get; set; }
        }

    }
}
