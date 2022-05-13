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
    public class GetIncomePerYear
    {
        public class Query : IRequest<IncomeResponsePerYear>
        {
            public string Month { get; set; }
            public int Year { get; set; }
        }

        public class IncomeResponsePerYear
        {
            public int TotalOrders { get; set; }
            public decimal TotalIncome { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IncomeResponsePerYear>
        {
            private readonly IAsyncDocumentSession _db;

            public QueryHandler(IAsyncDocumentSession db)
            {
                _db = db;
            }

            public async Task<IncomeResponsePerYear> Handle(Query request, CancellationToken cancellationToken)
            {
                var requestedStatistics = await _db.Query<OrderStatistics>()
                    .Where(x => x.Year == request.Year)
                    .FirstOrDefaultAsync();

                return new IncomeResponsePerYear
                {
                    TotalIncome = requestedStatistics?.TotalIncome ?? 0,
                    TotalOrders = requestedStatistics?.TotalOrders ?? 0,
                };

            }
        }
    }
}
