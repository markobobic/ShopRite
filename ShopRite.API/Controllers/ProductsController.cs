﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Core.Constants;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Responses;
using ShopRite.Platform.Products;
using System.Linq;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProduct.ProductRequest request)
        {
           await _mediator.Send(new CreateProduct.Command(request));
           return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string sortOrder, bool sortAscending, string search, string filter, int? pageNumber, int? limit)
        {
            var products = await _mediator.Send(new GetProducts.Query()
            {
                SortAscending = sortAscending,
                SortOrder = sortOrder,
                Search = search,
                PageNumber = pageNumber ?? PaginationConfig.FirstPageNumber,
                Limit = limit ?? PaginationConfig.DefaultLimit
            });
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _mediator.Send(new GetProduct.Query() { Id = id });
            if (product == null) return NotFound(new ApiResponse(404, "Product is not found."));
            return Ok(product);
        }

        [HttpGet("brands")]
        public IActionResult GetBrands() => Ok(ProductBrand.List.Select(x => x.Value).ToList());

        [HttpPut]
        public async Task<IActionResult> UpdateProd(UpdateProduct.ProductUpdateRequest request)
        {
            var product = await _mediator.Send(new UpdateProduct.Command(request));
            return Ok(product);
        }

    }
}
