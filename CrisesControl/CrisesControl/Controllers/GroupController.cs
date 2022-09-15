using CrisesControl.Api.Application.Commands.Groups.CheckGroup;
using CrisesControl.Api.Application.Commands.Groups.CreateGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetAllGroup;
using CrisesControl.Api.Application.Commands.Groups.SegregationLinks;
using CrisesControl.Api.Application.Commands.Groups.UpdateGroup;
using CrisesControl.Api.Application.Commands.Groups.GroupUpdateSegregationLink;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGroupQuery _groupQuery;

        public GroupController(IMediator mediator, IGroupQuery groupQuery)
        {
            _mediator = mediator;
            _groupQuery = groupQuery;
        }

        [HttpGet]
        [Route("{CompanyId:int}/{UserId:int}/{IncidentId:int}")]
        public async Task<IActionResult> GetAllGroup([FromRoute] GetAllGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{CompanyId:int}/{GroupId:int}")]
        public async Task<IActionResult> GetGroup([FromRoute] GetGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("SegregationLinks/{TargetID}/{LinkType}/{MemberShipType}")]
        public async Task<IActionResult> SegregationLinks([FromRoute] SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("CheckGroup/{CompanyId}/{GroupName}/{GroupId}")]
        public async Task<IActionResult> CheckGroup([FromRoute] CheckGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GroupUpdateSegregationLink")]
        public async Task<IActionResult> GroupUpdateSegregationLink([FromBody] GroupUpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
