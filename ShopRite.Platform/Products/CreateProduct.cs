using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Attributes;
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
            public Command(ProductRequest productRequest, IFormFile image)
            {
                ProductRequest = productRequest;
                Image = image;
            }

            public ProductRequest ProductRequest { get; set; }
            public IFormFile Image { get; }
        }
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ProductRequest.ProductJsonRequest.Name).NotEmpty();
                RuleFor(x => x.ProductRequest.ProductJsonRequest.ProductBrand).Must(ub =>
                {
                    ProductBrand.TryFromValue(ub, out var productBrand);
                    return productBrand != null && productBrand != string.Empty;
                })
                    .WithMessage("Product brand is not valid!");
                RuleFor(x => x.ProductRequest.ProductJsonRequest.ProductType).Must(ub =>
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
            private readonly IAwsService _awsService;
            public Handler(IDocumentStore db, IAwsService awsService)
            {
                _db = db;
                _awsService = awsService;
            }
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Image != null)
                    await _awsService.UploadImageToS3Bucket(request.Image);

                using var session = _db.OpenAsyncSession();
                await session.StoreAsync(new Product
                {
                    Price = request.ProductRequest.ProductJsonRequest.Price,
                    Description = request.ProductRequest.ProductJsonRequest.Description,
                    Name = request.ProductRequest.ProductJsonRequest.Name,
                    ProductBrand = request.ProductRequest.ProductJsonRequest.ProductBrand,
                    ImageUrl = request.Image == null ? null : _awsService.CreateUrlOfFile(request.Image),
                    ImagePreSignedUrl = request.Image == null ? null : _awsService.ReturnPreSignedURLOfUploadedImage(request.Image),
                    ProductType = request.ProductRequest.ProductJsonRequest.ProductType,
                    Stocks = request.ProductRequest.ProductJsonRequest.Stocks
                    .Select(x => new Stock { Size = x.Size, Quantity = x.Quantity }).ToList(),
                }, cancellationToken);

                await session.SaveChangesAsync();

                return new Response
                {
                    Price = request.ProductRequest.ProductJsonRequest.Price,
                    Description = request.ProductRequest.ProductJsonRequest.Description,
                    Name = request.ProductRequest.ProductJsonRequest.Name,
                    ProductType = request.ProductRequest.ProductJsonRequest.ProductType,
                    ProductBrand = request.ProductRequest.ProductJsonRequest.ProductBrand,
                    ImageUrl = _awsService.CreateUrlOfFile(request.Image),
                    ImagePreSignedUrl = request.Image == null ? null : _awsService.ReturnPreSignedURLOfUploadedImage(request.Image),
                    Stocks = request.ProductRequest.ProductJsonRequest.Stocks
                    .Select(x => new Stock { Size = x.Size, Quantity = x.Quantity }).ToList(),
                };
            }
            
        }
        public class ProductRequest
        {
            [FromJson]
            public ProductJsonRequest ProductJsonRequest { get; set; }
            public IFormFile Image { get; set; }
        }
        public class ProductJsonRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ProductType { get; set; }
            public string ProductBrand { get; set; }

            public List<StockDTO> Stocks { get; set; }
            public string ImageName { get; internal set; }

            public class StockDTO
            {
                public string Size { get; init; }
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
            public string ImageUrl { get; set; }
            public string ImagePreSignedUrl { get; set; }
        }
    }
}
