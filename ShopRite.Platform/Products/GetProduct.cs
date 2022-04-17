﻿using MediatR;
using Raven.Client.Documents;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class GetProduct
    {
        public class Query : IRequest<Response>
        {
            public string Id { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly IDocumentStore _db;

            public QueryHandler(IDocumentStore db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var product = await session.Query<Product>().FirstOrDefaultAsync(cancellationToken);
                
                return new Response
                {
                    Id = product.Id,
                    Description = product.Description,
                    Name = product.Name,
                    Price = product.Price,
                    Stocks = product.Stocks
                };
            }
        }

        public class Response
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
        }
    }
}
