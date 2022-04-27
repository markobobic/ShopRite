using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Extensions;
using ShopRite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class GetProducts
    {
        public class Query : IRequest<Response>
        {
            public string SortOrder { get; set; }
            public string Filter { get; set; }
            public bool SortAscending { get; set; }
            public string BrandName { get; set; }
            public string TypeName { get; set; }
            public string Search { get; set; }
            public int PageNumber { get; set; }
            public int Limit { get; set; }
        }

        public class Response
        {
            public IEnumerable<ProductDTO> Products { get; set; }
        }
        public class ProductDTO
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Brand { get; set; }
            public string Type { get; set; }
            public List<Stock> Stocks { get; set; }
            public string PictureUrl { get; internal set; }
            
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
                var products =  session.Query<Product>().ProjectInto<Product>();
                
                var sorts = new Dictionary<string, Expression<Func<Product, object>>>
                     {
                        {"price", x => x.Price},
                        {"name", x => x.Name},
                        {"brand", x => x.ProductBrand}
                    };

                var filter = new Dictionary<string, Expression<Func<Product, bool>>>
                     {
                        {"type", x => x.ProductType == ProductType.FromValue(request.TypeName)},
                        {"brand", x => x.ProductBrand == ProductBrand.FromValue(request.BrandName)}
                    };
                
                products = string.IsNullOrEmpty(request.Filter) ? products : products.Where(filter?.GetValueOrDefault(request.Filter));
                products = string.IsNullOrEmpty(request.Search) ? products : products.Search(c => c.Name, $"*{request.Search}*");
                
                if(request.SortOrder is not null)
                {
                    products = request.SortAscending ? products.OrderBy(sorts?.GetValueOrDefault(request.SortOrder))
                                                  : products.OrderByDescending(sorts?.GetValueOrDefault(request.SortOrder));
                }
                
                var productsToList = (await products.ToPagination(request.PageNumber, request.Limit, cancellationToken)).PaginatedList;

                return new Response
                {
                    Products = productsToList.Select(x => new ProductDTO
                    {
                        Price = x.Price,
                        Description = x.Description,
                        Name = x.Name,
                        Brand = x.ProductBrand,
                        Type = x.ProductType,
                        PictureUrl = x.PictureUrl,
                        Stocks = x.Stocks,
                    }).ToList()
                };

            }
        }
    }
}
