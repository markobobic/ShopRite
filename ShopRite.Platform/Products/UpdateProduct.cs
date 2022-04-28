using FluentValidation;
using MediatR;
using Raven.Client.Documents;
using ShopRite.Core.Enumerations;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Products
{
    public class UpdateProduct
    {
        public class Command : IRequest<Response>
        {
            public Command(ProductUpdateRequest request)
            {
                Request = request;
            }

            public ProductUpdateRequest Request { get; set; }
            
        }
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Request.Name).NotEmpty();
                RuleFor(x => x.Request.Brand).Must(ub =>
                {
                    ProductBrand.TryFromValue(ub, out var productBrand);
                    return productBrand != null && productBrand != string.Empty;
                })
                    .WithMessage("Product brand is not valid!");
                RuleFor(x => x.Request.Type).Must(ub =>
                {
                    ProductType.TryFromValue(ub, out var productType);
                    return productType != null && productType != string.Empty;
                })
                    .WithMessage("Product type is not valid!");
            }
        }
        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IDocumentStore _db;

            public Handler(IDocumentStore db)
            {
                _db = db;
            }
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                using var session = _db.OpenAsyncSession();
                var existingProduct = await session.LoadAsync<Product>(request.Request.Id, cancellationToken);
                session.Advanced.Evict(existingProduct);
                var excludedStocks = existingProduct.Stocks.Where(s => !request.Request.Stocks.Any(p => p.Id == s.Id)).ToList();
                excludedStocks.AddRange(request.Request.Stocks);
                existingProduct.Stocks = excludedStocks.OrderBy(o => o.Id).ToList();

                var product = existingProduct with
                {
                    Id = request.Request.Id,
                    Name = request.Request.Name,
                    Description = request.Request.Description,
                    Price = request.Request.Price,
                    ImageUrl = request.Request.PictureUrl,
                    ProductBrand = request.Request.Brand,
                    ProductType = request.Request.Type,
                    Stocks = existingProduct.Stocks
                };
                await session.StoreAsync(product);
                await session.SaveChangesAsync(cancellationToken);
                
                return new Response
                {
                    Description = product.Description,
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stocks = existingProduct.Stocks,
                    PictureUrl = product.ImageUrl,
                    Type = product.ProductType,
                    Brand = product.ProductBrand
                };
            }
        }
        public class ProductUpdateRequest
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
            public string Brand { get; internal set; }
            public string Type { get; internal set; }
            public string PictureUrl { get; internal set; }
        }

        public class Response
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
            public string PictureUrl { get; internal set; }
            public string Type { get; internal set; }
            public string Brand { get; internal set; }
        }
    }
}
