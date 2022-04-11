using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Stocks
{
    public class DeleteStock
    {
        public class Command : IRequest<bool>
        {
            public string Id { get; set; }
            public Command(string id)
            {
                Id = id;
            }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IDocumentStore _db;
            public Handler(IDocumentStore db)
            {
                _db = db;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var stock = await session.Query<Stock>().Include(x => x.ProductId).FirstOrDefaultAsync();
                session.Delete(stock);
                await session.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
