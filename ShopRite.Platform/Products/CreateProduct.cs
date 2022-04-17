using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class CreateProduct
    {
        public class Command : IRequest<Response>
        {
            public Command(ProductRequest productRequest)
            {
                ProductRequest = productRequest;
            }

            public ProductRequest ProductRequest { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IDocumentStore _db;

            public Handler(IDocumentStore db)
            {
                _db = db;
            }
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                await session.StoreAsync(new Product
                {
                    Price = request.ProductRequest.Price,
                    Description = request.ProductRequest.Description,
                    Name = request.ProductRequest.Name,
                    Stocks = request.ProductRequest.Stocks,
                }, cancellationToken);
                
                await session.SaveChangesAsync();
                
                return new Response
                {
                    Price = request.ProductRequest.Price,
                    Description = request.ProductRequest.Description,
                    Name = request.ProductRequest.Name,
                    Stocks = request.ProductRequest?.Stocks,
                };
            }
        }

        public class ProductRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
        }
        public class Response
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }

        }
    }
}
