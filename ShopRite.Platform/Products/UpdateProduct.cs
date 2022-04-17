using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class UpdateProduct
    {
        public class Command : IRequest<Response>
        {
            public Command(ProductUpdateRequest request)
            {
                Request = request;
            }

            public ProductUpdateRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IDocumentStore _db;

            public Handler(IDocumentStore db)
            {
                _db = db;
            }
            // Stock { id: 1  name: Nike Quantity: 30}  StockUpdate {id:1 name:Addidas} 
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var existingProduct = await session.LoadAsync<Product>(request.Request.Id, cancellationToken);
                session.Advanced.Evict(existingProduct);
                var excludedStocks = existingProduct.Stocks.Where(s => !request.Request.Stocks.Any(p => p.Id == s.Id)).ToList();
                excludedStocks.AddRange(request.Request.Stocks);
                existingProduct.Stocks = excludedStocks.OrderBy(o => o.Id).ToList();

                var product = existingProduct with
                {
                    Id = request.Request.Id,
                    Name = request.Request.Name,
                    Description = request.Request.Description,
                    Price = request.Request.Price,
                    Stocks = existingProduct.Stocks
                };
                await session.StoreAsync(product);
                await session.SaveChangesAsync(cancellationToken);
                return new Response
                {
                    Description = product.Description,
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stocks = existingProduct.Stocks
                };
            }
        }
        public class ProductUpdateRequest
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
        }

        public class Response
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
        }
    }
}
