using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.PostCompanies
{
    public class GetPostCompanies
    {
        public class Query : IRequest<List<CompaniesAllResponse>> { }

        public class QueryHandler : IRequestHandler<Query, List<CompaniesAllResponse>>
        {
            private readonly IAsyncDocumentSession _db;

            public QueryHandler(IAsyncDocumentSession db)
            {
                _db = db;
            }
            public async Task<List<CompaniesAllResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var postCompanies = await _db.Query<PostCompany>().ToListAsync();
                //TODO Return all post companies 
                return null;
            }
        }
        public class CompaniesAllResponse
        {
            public string Name { get; set; }
            public DeliveryMethod DeliveryMethod { get; set; }
            public string FullAddress { get; set; }
            public string PhoneNumber { get; set; }
            public int Rating { get; set; }
        }
    }
}
