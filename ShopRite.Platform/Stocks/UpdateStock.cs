using Ardalis.GuardClauses;
using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Stocks
{
    public class UpdateStock
    {
        public class Command : IRequest<Response>
        {
            public Command(Request request)
            {
                Request = request;
            }

            public Request Request { get; }
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
                var stocks = new List<StockUpdateDTO>();
                foreach (var stockDto in request.Request.Stocks)
                {
                    var stockToUpdate = await session.Query<Stock>().Include(x => x.ProductId).FirstAsync(x => x.Id == stockDto.Id);
                    if (stockToUpdate is null) Guard.Against.Null(stockToUpdate, nameof(stockToUpdate), "Invalid stock.");
                    session.Advanced.Evict(stockToUpdate);
                    stockToUpdate = stockToUpdate with
                    {
                        Id = stockDto.Id,
                        Description = stockDto.Description,
                        ProductId = stockDto.ProductId,
                        Quantity = stockDto.Quantity,
                    };
                    stocks.Add(stockDto);
                    await session.StoreAsync(stockToUpdate);
                }
                await session.SaveChangesAsync();
                
                return new Response
                {
                    Stocks = stocks
                };
                
            }
        }

        public class Request
        {
            public IEnumerable<StockUpdateDTO> Stocks { get; set; }
        }
        public class StockUpdateDTO
        {
            public string Id { get; set; }
            public string ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
        }

        public class Response
        {
            public IEnumerable<StockUpdateDTO> Stocks { get; set; }
        }
    }

    
}
