using Amazon.S3;
using Amazon.S3.Model;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using ShopRite.Core.Configurations;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Extensions;
using ShopRite.Domain;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Attributes;
using System;
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
            private readonly IAmazonS3 _s3Client;
            private readonly IConfiguration _configuration;
            private readonly AWSConfig _awsConfig;

            public Handler(IDocumentStore db, IAmazonS3 s3Client, IConfiguration configuration)
            {
                _db = db;
                _s3Client = s3Client;
                _configuration = configuration;
                _awsConfig = _configuration.Get<AWSConfig>();
            }
            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var urlOfFile = await UploadImageToS3Bucket(request);
                using var session = _db.OpenAsyncSession();
                await session.StoreAsync(new Product
                {
                    Price = request.ProductRequest.ProductJsonRequest.Price,
                    Description = request.ProductRequest.ProductJsonRequest.Description,
                    Name = request.ProductRequest.ProductJsonRequest.Name,
                    ProductBrand = request.ProductRequest.ProductJsonRequest.ProductBrand,
                    ImageUrl = urlOfFile,
                    ProductType = request.ProductRequest.ProductJsonRequest.ProductType,
                    Stocks = request.ProductRequest.ProductJsonRequest.Stocks
                    .Select(x => new Stock { Description = x.Description, Quantity = x.Quantity }).ToList(),
                }, cancellationToken);

                await session.SaveChangesAsync();

                return new Response
                {
                    Price = request.ProductRequest.ProductJsonRequest.Price,
                    Description = request.ProductRequest.ProductJsonRequest.Description,
                    Name = request.ProductRequest.ProductJsonRequest.Name,
                    ProductType = request.ProductRequest.ProductJsonRequest.ProductType,
                    ProductBrand = request.ProductRequest.ProductJsonRequest.ProductBrand,
                    Stocks = request.ProductRequest.ProductJsonRequest.Stocks
                    .Select(x => new Stock { Description = x.Description, Quantity = x.Quantity }).ToList(),
                };
            }

            private async Task<string> UploadImageToS3Bucket(Command request)
            {
                var awsRequest = new PutObjectRequest()
                {
                    BucketName = _awsConfig.AWS.BucketName,
                    Key = request.Image.FileName,
                    InputStream = request.Image.OpenReadStream()
                };
                awsRequest.Metadata.Add("Content-Type", request.Image.ContentType);
                var response = await _s3Client.PutObjectAsync(awsRequest);
                Guard.Against.False(response.HttpStatusCode.ToInt() == 200, "S3 bucket didn't upload file.");
                
                GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest();
                urlRequest.BucketName = _awsConfig.AWS.BucketName;
                urlRequest.Key = request.Image.FileName;
                urlRequest.Expires = DateTime.Now.AddHours(1);
                urlRequest.Protocol = Protocol.HTTP;
                string url = _s3Client.GetPreSignedURL(urlRequest);
               
                return url;
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
