using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
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
        public class Query : IRequest<PaginatedList<ProductDTO>>
        {
            public string SortOrder { get; set; }
            public string Filter { get; set; }
            public bool SortAscending { get; set; }
            public string BrandName { get; set; }
            public string TypeName { get; set; }
            public string Search { get; set; }
            public int PageNumber { get; set; }
            public int Limit { get; set; }
            public int? AmountTo { get; set; }
            public int? AmountFrom { get; set; }
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
            public string ImageUrl { get; set; }
            public string ImagePreSignedUrl { get; set; }

        }

        public class QueryHandler : IRequestHandler<Query, PaginatedList<ProductDTO>>
        {
            private readonly IDocumentStore _db;

            public QueryHandler(IDocumentStore db)
            {
                _db = db;
            }
            public async Task<PaginatedList<ProductDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var products = session.Query<Product>().ProjectInto<Product>();

                Dictionary<string, Expression<Func<Product, object>>> sorts;
                Dictionary<string, Expression<Func<Product, bool>>> filter;
                
                SetSearchFilters(request, out sorts, out filter);

                products = string.IsNullOrEmpty(request.Filter) ?
                    products : products.Where(filter?.GetValueOrDefault(request.Filter));

                products = string.IsNullOrEmpty(request.Search) ?
                    products : products.Search(c => c.Name, $"*{request.Search}*");

                if (request.SortOrder is not null)
                {
                    products = request.SortAscending ? products.OrderBy(sorts?.GetValueOrDefault(request.SortOrder))
                                                  : products.OrderByDescending(sorts?.GetValueOrDefault(request.SortOrder));
                }

                var productsToList = (await products.ToPagination(request.PageNumber, request.Limit, cancellationToken));

                return new PaginatedList<ProductDTO>
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.Limit,
                    TotalItems = productsToList.QueryStatistics.TotalResults,
                    Data = productsToList.PaginatedList
                    .Where(x => x.Price >= request.AmountFrom && x.Price <= request.AmountTo)
                    .Select(x => new ProductDTO
                    {
                        Price = x.Price,
                        Description = x.Description,
                        Name = x.Name,
                        Brand = x.ProductBrand,
                        Type = x.ProductType,
                        ImagePreSignedUrl = x.ImagePreSignedUrl,
                        ImageUrl = x.ImageUrl,
                        Stocks = x.Stocks,
                    }).ToList()

                };

            }

            private void SetSearchFilters(Query request, out Dictionary<string, Expression<Func<Product, object>>> sorts, out Dictionary<string, Expression<Func<Product, bool>>> filter)
            {
                sorts = new Dictionary<string, Expression<Func<Product, object>>>
                     {
                        {"price", x => x.Price},
                        {"name", x => x.Name},
                        {"brand", x => x.ProductBrand}
                    };
                filter = new Dictionary<string, Expression<Func<Product, bool>>>
                     {
                        {"type", x => x.ProductType == ProductType.FromValue(request.TypeName)},
                        {"brand", x => x.ProductBrand == ProductBrand.FromValue(request.BrandName)}
                    };
            }
        }
    }
}
