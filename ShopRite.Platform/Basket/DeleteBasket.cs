using MediatR;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Basket
{
    public class DeleteBasket
    {
        public class Command : IRequest<bool>
        {
            public BasketDeleteRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IDatabase _db;
            public Handler(IConnectionMultiplexer db)
            {
                _db = db.GetDatabase();
            }
            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                return await _db.KeyDeleteAsync(request.Request.BasketId);
            }
        }
        public class BasketDeleteRequest
        {
            public string BasketId { get; set; }
        }

    }
}
