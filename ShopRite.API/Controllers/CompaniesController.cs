using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Platform.PostCompanies;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CompaniesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public IActionResult CreateCompany(CreatePostCompany.PostCompanyRequest request)
        {
            var response = _mediator.Send(new CreatePostCompany.Command(request));
            return Ok(response);
        }
    }
}
