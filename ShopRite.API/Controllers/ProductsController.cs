using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Core.Enumerations;
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
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _mediator.Send(new GetProducts.Query());
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllProducts(string id)
        {
            var product = await _mediator.Send(new GetProduct.Query() { Id = id });
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
