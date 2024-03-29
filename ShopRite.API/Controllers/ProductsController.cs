﻿using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Core.Constants;
using ShopRite.Core.DTOs;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProduct.ProductRequest request)
        {
           var response = await _mediator.Send(new CreateProduct.Command(request, request.Image));
           return Ok(response);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParams parametars)
        {
            var products = await _mediator.Send(new GetProducts.Query()
            {
                SortAscending = parametars.SortAscending,
                SortOrder = parametars.SortOrder,
                Search = parametars.Search,
                PageNumber = parametars.PageNumber ?? PaginationConfig.FirstPageNumber,
                Limit = parametars.Limit ?? PaginationConfig.DefaultLimit,
                AmountFrom = parametars.AmountFrom ?? int.MinValue,
                AmountTo = parametars.AmountTo ?? int.MaxValue,
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> UpdateProd(UpdateProduct.ProductUpdateRequest request)
        {
            var product = await _mediator.Send(new UpdateProduct.Command(request));
            return Ok(product);
        }

    }
}
