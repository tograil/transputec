using AutoMapper;
using CrisesControl.Api.Application.Commands.CustomEventLog.ExportEventLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLogHeader;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetLogs;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetMessageLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLogHeader;
using CrisesControl.Api.Application.Commands.CustomEventLog.SaveLogMessage;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class CustomEventLogController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICustomEventLogQuery _customEventLogQuery;

        public CustomEventLogController(IMediator mediator, ICustomEventLogQuery customEventLogQuery)
        {
            _mediator = mediator;
            _customEventLogQuery = customEventLogQuery;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEventLog/{EventLogId:int}/{EventLogHeaderId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetEventLog([FromRoute] GetEventLogRequest request)
        {
            var result = await _customEventLogQuery.GetEventLog(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEventLogHeader/{ActiveIncidentId:int}/{EventLogHeaderId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetEventLogHeader([FromRoute] GetEventLogHeaderRequest request)
        {
            var result = _customEventLogQuery.GetEventLogHeader(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLogs/{ActiveIncidentId:int}/{EventLogHeaderId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetLogs([FromRoute] GetLogsRequest request)
        {
            var result = await _customEventLogQuery.GetLogs(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageLog/{EventLogId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetMessageLog([FromRoute] GetMessageLogRequest request)
        {
            var result = await _customEventLogQuery.GetMessageLog(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveEventLogHeader")]
        public async Task<IActionResult> SaveEventLogHeader([FromBody] SaveEventLogHeaderRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveEventLog")]
        public async Task<IActionResult> SaveEventLog([FromBody] SaveEventLogRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveLogMessage")]
        public async Task<IActionResult> SaveLogMessage([FromBody] SaveLogMessageRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("ExportEventLog")]
        public async Task<IActionResult> ExportEventLog([FromBody] ExportEventLogRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
