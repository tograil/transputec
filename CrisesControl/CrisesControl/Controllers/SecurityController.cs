using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISecurityQuery _securityQuery;

        public SecurityController(IMediator mediator, ISecurityQuery securityQuery)
        {
            this._mediator = mediator;
            this._securityQuery = securityQuery;
        }
        [HttpGet]
        [Route("GetCompanySecurityGroup/{CompanyID:int}")]
        public async Task<IActionResult> GetCompanySecurityGroup([FromRoute] GetCompanySecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
