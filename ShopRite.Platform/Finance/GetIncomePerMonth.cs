using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Finance
{
    public class GetIncomePerMonth
    {
        public class Query : IRequest<IncomeResponse>
        {
            public string Month { get; set; }
            public int Year { get; set; }
        }

        public class IncomeResponse
        {
            public int TotalOrders { get; set; }
            public decimal TotalIncome { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IncomeResponse>
        {
            private readonly IAsyncDocumentSession _db;

            public QueryHandler(IAsyncDocumentSession db)
            {
                _db = db;
            }

            public async Task<IncomeResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var requestedStatistics = await _db.Query<OrderStatistics>()
                    .Where(x => string.Equals(x.Month, request.Month, StringComparison.OrdinalIgnoreCase) && x.Year == request.Year)
                    .FirstOrDefaultAsync();
                
                return new IncomeResponse
                {
                    TotalIncome = requestedStatistics?.TotalIncome ?? 0,
                    TotalOrders = requestedStatistics?.TotalOrders ?? 0,
                };
                    
            }
        }
    }
}
