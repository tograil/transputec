using CrisesControl.Api.Application.Commands.Groups.CreateGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroups;
using CrisesControl.Api.Application.Commands.Groups.UpdateGroup;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GroupController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IGroupQuery _groupQuery;

        public GroupController(IMediator mediator, IGroupQuery groupQuery)
        {
            _mediator = mediator;
            _groupQuery = groupQuery;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] GetGroupsRequest request, CancellationToken cancellationToken)
        {
            var result = await _groupQuery.GetGroups(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetGroup([FromQuery] GetGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _groupQuery.GetGroup(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
