using Ardalis.GuardClauses;
using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Stocks
{
    public class CreateStock
    {
        public class Command : IRequest<Response>
        {
            public Command(StockCreateDTO request)
            {
                Request = request;
            }

            public StockCreateDTO Request { get; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IDocumentStore db;

            public Handler(IDocumentStore db)
            {
                this.db = db;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = db.OpenAsyncSession();
                var product = await session.LoadAsync<Product>(request.Request.ProductId);
                Guard.Against.Null(product, nameof(product), "Product doesn't exist.");
                
                var stock = new Stock()
                {
                    Description = request.Request.Description,
                    ProductId = request.Request.ProductId,
                    Quantity = request.Request.Quantity,
                
                };
                await session.StoreAsync(stock);
                await session.SaveChangesAsync(cancellationToken);
                
                return new Response
                {
                    Description = stock.Description,
                    ProductId = stock.ProductId,
                    Quantity = stock.Quantity,
                };
            }
        }

        public class StockCreateDTO
        {
            public string ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
        }
        public class Response
        {
            public string ProductId { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
        }
    }
}
