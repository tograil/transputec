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
        /// <summary>
        /// Get all groups list
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        [HttpGet]
        [Route("{CompanyId:int}/{IncidentId:int}/{FilterVirtual}")]
        public async Task<IActionResult> GetAllGroup([FromRoute] GetAllGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _groupQuery.GetAllGroup(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{CompanyId:int}/{GroupId:int}")]
        public async Task<IActionResult> GetGroup([FromRoute] GetGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Update group
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Return segregationlinks based on type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SegregationLinks/{TargetID}/{LinkType}/{MemberShipType}")]
        public async Task<IActionResult> SegregationLinks([FromRoute] SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Check the existance of group
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckGroup/{CompanyId}/{GroupName}/{GroupId}")]
        public async Task<IActionResult> CheckGroup([FromRoute] CheckGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Create group segregation link
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GroupUpdateSegregationLink")]
        public async Task<IActionResult> GroupUpdateSegregationLink([FromBody] GroupUpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
