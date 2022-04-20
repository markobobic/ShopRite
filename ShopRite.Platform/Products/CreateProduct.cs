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
    public class CreateProduct
    {
        public class Command : IRequest<Response>
        {
            public Command(ProductRequest productRequest)
            {
                ProductRequest = productRequest;
            }

            public ProductRequest ProductRequest { get; set; }
        }
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ProductRequest.Name).NotEmpty();
                RuleFor(x => x.ProductRequest.ProductBrand).Must(ub =>
                {
                    ProductBrand.TryFromValue(ub, out var productBrand);
                    return productBrand != null && productBrand != string.Empty;
                })
                    .WithMessage("Product brand is not valid!");
                RuleFor(x => x.ProductRequest.ProductType).Must(ub =>
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
                await session.StoreAsync(new Product
                {
                    Price = request.ProductRequest.Price,
                    Description = request.ProductRequest.Description,
                    Name = request.ProductRequest.Name,
                    ProductBrand = request.ProductRequest.ProductBrand,
                    PictureUrl = request.ProductRequest.PictureUrl,
                    ProductType = request.ProductRequest.ProductType,
                    Stocks = request.ProductRequest.Stocks
                    .Select(x => new Stock {Description = x.Description, Quantity = x.Quantity }).ToList(),
                }, cancellationToken);
                
                await session.SaveChangesAsync();
                
                return new Response
                {
                    Price = request.ProductRequest.Price,
                    Description = request.ProductRequest.Description,
                    Name = request.ProductRequest.Name,
                    ProductType = request.ProductRequest.ProductType,
                    ProductBrand = request.ProductRequest.ProductBrand,
                    Stocks = request.ProductRequest.Stocks
                    .Select(x => new Stock { Description = x.Description, Quantity = x.Quantity }).ToList(),
                };
            }
        }

        public class ProductRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ProductType { get; set; }
            public string ProductBrand { get; set; }

            public List<StockDTO> Stocks { get; set; }
            public string PictureUrl { get; internal set; }

            public class StockDTO
            {
                public string Description { get; init; }
                public int Quantity { get; init; }
            }
        }
        public class Response
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public List<Stock> Stocks { get; set; }
            public string ProductType { get; internal set; }
            public string ProductBrand { get; internal set; }
        }
    }
}
