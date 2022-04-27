using Ardalis.GuardClauses;
using MediatR;
using ShopRite.Domain;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Basket
{
    public class UpdateBasket
    {
        public class Command : IRequest<Unit>
        {
            public BasketUpdateRequest Request { get; set; }
        }
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IDatabase _db;
            public Handler(IConnectionMultiplexer db)
            {
                _db = db.GetDatabase();
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var created = await _db.
                    StringSetAsync(request.Request.CustomerBasket.Id,
                    JsonSerializer.Serialize(request.Request.CustomerBasket),
                    TimeSpan.FromDays(30));
                
                Guard.Against.AgainstExpression(x => created == false, created, "Basket is not created");
               
                return Unit.Value;
            }
        }
        public class BasketUpdateRequest
        {
            public CustomerBasket CustomerBasket { get; set; }
        }
        
    }
}
