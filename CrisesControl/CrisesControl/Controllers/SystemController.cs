using CrisesControl.Api.Application.Commands.System.ApiStatus;
using CrisesControl.Api.Application.Commands.System.CleanLoadTestResult;
using CrisesControl.Api.Application.Commands.System.CompanyStatsAdmin;
using CrisesControl.Api.Application.Commands.System.DownloadExportFile;
using CrisesControl.Api.Application.Commands.System.ExportCompanyData;
using CrisesControl.Api.Application.Commands.System.ExportTrackingData;
using CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId;
using CrisesControl.Api.Application.Commands.System.PushCMLog;
using CrisesControl.Api.Application.Commands.System.PushTwilioLog;
using CrisesControl.Api.Application.Commands.System.TwilioLogDump;
using CrisesControl.Api.Application.Commands.System.ViewErrorLog;
using CrisesControl.Api.Application.Commands.System.ViewModelLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("ExportTrackingData/{TrackMeID}/{UserDeviceID}/{StartDate}/{EndDate}")]
        public async Task<IActionResult> ExportTrackingData([FromRoute] ExportTrackingDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("ViewModelLog/{StartDate}/{EndDate}/{draw}")]
        public async Task<IActionResult> ViewModelLog([FromRoute] ViewModelLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get Audit log By Record ID
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAuditLogsByRecordId/{RecordId:int}/{TableName}/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetAuditLogsByRecordId([FromRoute] GetAuditLogsByRecordIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Export COmpany Data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("ExportCompanyData/{Entity}")]
        //public async Task<IActionResult> ExportCompanyData([FromRoute] ExportCompanyDataRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);

        //    return Ok(result);
        //}
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadExportFile/CompanyId={CompanyId}&FileName={FileName}")]
        public async Task<IActionResult> DownloadExportFile([FromRoute] DownloadExportFileRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CompanyStatsAdmin")]
        public async Task<IActionResult> CompanyStatsAdmin([FromRoute] CompanyStatsAdminRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// View Error Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ViewErrorLog")]
        public async Task<IActionResult> ViewErrorLog([FromRoute] ViewErrorLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Api Status
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ApiStatus")]
        public async Task<IActionResult> ApiStatus([FromRoute] ApiStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Twilio Log Dump
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //TODO::It gives error
        //[HttpGet]
        //[Route("TwilioLogDump/{LogType}")]
        //public async Task<IActionResult> TwilioLogDump([FromRoute] TwilioLogDumpRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);

        //    return Ok(result);
        //}
        /// <summary>
        /// Push Twilio Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PushTwilioLog/Method={Method}&Sid={Sid}")]
        public async Task<IActionResult> PushTwilioLog([FromRoute] PushTwilioLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PushCMLog/Method={Method}&Sid={Sid}")]
        public async Task<IActionResult> PushCMLog([FromRoute] PushCMLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CleanLoadTestResult")]
        public async Task<IActionResult> CleanLoadTestResult([FromRoute] CleanLoadTestResultRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
