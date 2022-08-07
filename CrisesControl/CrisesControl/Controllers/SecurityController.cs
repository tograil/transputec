using CrisesControl.Api.Application.Commands.Security.AddSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects;
using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup;
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

        [HttpGet]
        [Route("[action]/{SecurityGroupId:int}")]
        public async Task<IActionResult> GetSecurityGroup([FromRoute] GetSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllSecurityObjects([FromRoute] GetAllSecurityObjectsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddSecurityGroup([FromBody] AddSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateSecurityGroup([FromBody] UpdateSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        [Route("[action]/{SecurityGroupId}")]
        public async Task<IActionResult> DeleteSecurityGroup([FromRoute] DeleteSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
