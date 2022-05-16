using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Finance
{
    public class GetIncomePerMonth
    {
        public class Query : IRequest<IncomeResponsePerMonth>
        {
            public string Month { get; set; }
            public int Year { get; set; }
        }

        public class IncomeResponsePerMonth
        {
            public int TotalOrders { get; set; }
            public decimal TotalIncome { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IncomeResponsePerMonth>
        {
            private readonly IAsyncDocumentSession _db;

            public QueryHandler(IAsyncDocumentSession db)
            {
                _db = db;
            }

            public async Task<IncomeResponsePerMonth> Handle(Query request, CancellationToken cancellationToken)
            {
                var requestedStatistics = await _db.Query<OrderStatistics>()
                    .Where(x => string.Equals(x.Month, request.Month, StringComparison.OrdinalIgnoreCase) && x.Year == request.Year)
                    .FirstOrDefaultAsync();
                
                return new IncomeResponsePerMonth
                {
                    TotalIncome = requestedStatistics?.TotalIncome ?? 0,
                    TotalOrders = requestedStatistics?.TotalOrders ?? 0,
                };
                    
            }
        }
    }
}
