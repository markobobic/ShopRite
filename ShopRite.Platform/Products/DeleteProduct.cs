using MediatR;
using Raven.Client.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class DeleteProduct
    {
        public class Command : IRequest<Unit>
        {
            public Command(string productId)
            {
                ProductId = productId;
            }
            public string ProductId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IDocumentStore _db;
            public Handler(IDocumentStore db)
            {
                _db = db;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                session.Delete(request.ProductId);
                await session.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
