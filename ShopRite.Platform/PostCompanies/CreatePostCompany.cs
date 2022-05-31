using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.PostCompanies
{
    public class CreatePostCompany
    {
        public class Command : IRequest<CreatePostCompanyResponse>
        {
            public Command(PostCompanyRequest postCompanyRequest)
            {
                PostCompanyRequest = postCompanyRequest;
            }

            public PostCompanyRequest PostCompanyRequest { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, CreatePostCompanyResponse>
        {
            private readonly IDocumentStore _db;
            public Handler(IDocumentStore db)
            {
                _db = db;
            }
            public async Task<CreatePostCompanyResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                await session.StoreAsync(new PostCompany
                {
                    Name = request.PostCompanyRequest.Name,
                    FullAddress = request.PostCompanyRequest.FullAddress,
                    DeliveryMethods = request.PostCompanyRequest.DeliveryMethods,
                    PhoneNumber = request.PostCompanyRequest.PhoneNumber,
                    Rating = 0,
                });

                await session.SaveChangesAsync();

                return new CreatePostCompanyResponse
                {
                    Name = request.PostCompanyRequest.Name,
                    FullAddress = request.PostCompanyRequest.FullAddress,
                    PhoneNumber = request.PostCompanyRequest.PhoneNumber,
                    Rating = 0,
                };
            }

        }
        public class PostCompanyRequest
        {
            public string Name { get; set; }
            public Dictionary<DistanceType, DeliveryMethod> DeliveryMethods { get; set; }
            public string FullAddress { get; set; }
            public string PhoneNumber { get; set; }
        }
        public class CreatePostCompanyResponse
        {
            public string Name { get; set; }
            public Dictionary<DistanceType, DeliveryMethod> DeliveryMethods { get; set; }
            public string FullAddress { get; set; }
            public string PhoneNumber { get; set; }
            public int Rating { get; set; }
        }
    }
}
