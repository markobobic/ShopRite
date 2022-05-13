using MediatR;
using Raven.Client.Documents;
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
        public class Query : IRequest<IncomeResponse> { }

        public class IncomeResponse
        {
        }

        public class QueryHandler : IRequestHandler<Query, IncomeResponse>
        {
            private readonly IDocumentStore _db;

            public QueryHandler(IDocumentStore db)
            {
                _db = db;
            }

            public async Task<IncomeResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                return null;
            }
        }
    }
}
