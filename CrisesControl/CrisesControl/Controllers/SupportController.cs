using CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.Support.GetIncidentData;
using CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Support.GetIncidentReportDetails;
using CrisesControl.Api.Application.Commands.Support.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Support.GetUser;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Compatibility;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SupportController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ISupportQuery _supportQuery;
        public SupportController(IMediator mediator, ISupportQuery supportQuery)
        {
            _mediator = mediator;
            _supportQuery = supportQuery;
        }

        [HttpGet]
        [Route("GetIncidentData/{IncidentActivationId:int}/{OutUserCompanyId:int}")]
        public async Task<IActionResult> GetIncidentData([FromRoute] GetIncidentDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.GetIncidentData(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentStats/{IncidentActivationId:int}/{OutUserCompanyId:int}")]
        public async Task<IActionResult> GetIncidentStats([FromRoute] GetIncidentStatsRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.GetIncidentStats(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentReportDetails/{IncidentActivationId:int}/{OutUserCompanyId:int}")]
        public async Task<IActionResult> GetIncidentReportDetails([FromRoute] GetIncidentReportDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.GetIncidentReportDetails(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("ActiveIncidentTasks/{ActiveIncidentId:int}/{OutUserCompanyId:int}")]
        public async Task<IActionResult> ActiveIncidentTasks([FromRoute] ActiveIncidentTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.ActiveIncidentTasks(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentMessageAck")]
        public async Task<IActionResult> GetIncidentMessageAck([FromBody] GetIncidentMessageAckRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.GetIncidentMessageAck(request);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetUser/{UserId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] GetUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _supportQuery.GetUser(request);

            return Ok(result);
        }
    }
}
