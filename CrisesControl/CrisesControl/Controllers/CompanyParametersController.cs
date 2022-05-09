using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyParametersController : ControllerBase {
        private readonly IMediator _mediator;
        private readonly ICompanyParametersQuery _companyParametersQuery;

        public CompanyParametersController(IMediator mediator, ICompanyParametersQuery companyParametersQuery) {
            this._mediator = mediator;
            this._companyParametersQuery = companyParametersQuery;
        }
        [HttpGet]
        [Route("GetCascading/{CompanyID:int}/{PlanType:string}/{PlanID:int}")]
        public async Task<IActionResult> GetCascading([FromRoute] GetCascadingRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetCompanyFTP/{CompanyId:int}")]
        public async Task<IActionResult> GetCompanyFTP([FromRoute] GetCompanyFTPRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get all the company parameters by company id.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllCompanyParameters/{CompanyId:int}")]
        public async Task<IActionResult> GetAllCompanyParameters([FromRoute] GetAllCompanyParametersRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
