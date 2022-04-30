using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class GetProduct
    {
        public class Query : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IDocumentStore _db;

            public QueryHandler(IDocumentStore db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var product = await session.Query<Product>().FirstOrDefaultAsync(x => x.Id == request.Id,cancellationToken);
                if (product == null) return null;

                return new Response
                {
                    Id = product.Id,
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Brand = product.ProductBrand,
                    Type = product.ProductType,
                    ImagePreSignedUrl = product.ImagePreSignedUrl,
                    ImageUrl = product.ImageUrl,
                    Stocks = product.Stocks,
                };
            }
        }

        public class Response
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Brand { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
            public string ImagePreSignedUrl { get; set; }
            public List<Stock> Stocks { get; set; }
        }
    }
}
