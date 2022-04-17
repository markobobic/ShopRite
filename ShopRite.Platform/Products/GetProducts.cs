using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class GetProducts
    {
        public class Query : IRequest<Response>
        {

        }

        public class Response
        {
            public IEnumerable<ProductDTO> Products { get; set; }
        }
        public class ProductDTO
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
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
                return new Response
                {
                    Products = (await session.Query<Product>().ToListAsync()).Select(x => new ProductDTO
                    {
                        Price = x.Price,
                        Description = x.Description,
                        Name = x.Name,
                        Stocks = x.Stocks,
                    }).ToList()
                };

            }
        }
    }
}
