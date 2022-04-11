using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Stocks
{
    public class GetStocks
    {
        public class Query : IRequest<Response> { }

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
                return new Response()
                {
                    Stocks = await session.Query<Stock>().Include(x => x.ProductId).Select(x => new StockGetDTO
                    {
                        ProductId = x.ProductId,
                        Description = x.Description,
                        Quantity = x.Quantity
                    }).ToListAsync(cancellationToken)
                };
            }
        }
        public class StockGetDTO
        {
            public string ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
        }
        public class Response
        {
            public IEnumerable<StockGetDTO> Stocks { get; set; }
        }
    }
}
