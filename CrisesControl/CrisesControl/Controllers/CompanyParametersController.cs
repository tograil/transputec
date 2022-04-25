using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyParametersController : Controller {
        private readonly IMediator _mediator;

        public CompanyParametersController(IMediator mediator) {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all the company parameters by company id.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{CompanyId:int}")]
        public async Task<IActionResult> GetAllCompanyParameters([FromRoute] GetAllCompanyParametersRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
