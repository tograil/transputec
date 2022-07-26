
using CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.AddNotes;
using CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS;
using CrisesControl.Api.Application.Commands.Incidents.CloneIncident;
using CrisesControl.Api.Application.Commands.Incidents.CopyIncident;
using CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic;
using CrisesControl.Api.Application.Commands.Incidents.GetAttachments;
using CrisesControl.Api.Application.Commands.Incidents.GetCallToAction;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes;
using CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline;
using CrisesControl.Api.Application.Commands.Incidents.InitiateAndLaunchIncident;
using CrisesControl.Api.Application.Commands.Incidents.InitiateCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.LaunchCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.UpdateSOS;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Compatibility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentController : Controller
{
    private readonly IMediator _mediator;
    private readonly IIncidentQuery _incidentQuery;

    public IncidentController(IMediator mediator,
        IIncidentQuery incidentQuery)
    {
        _mediator = mediator;
        _incidentQuery = incidentQuery;

    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> AddCompanyIncident([FromBody] AddCompanyIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CloneIncident([FromBody] CloneIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SetupCompleted([FromBody] CopyIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> InitiateCompanyIncident([FromBody] InitiateCompanyIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> LaunchCompanyIncident([FromBody] LaunchCompanyIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> InitiateAndLaunchIncident([FromBody] InitiateAndLaunchIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get all Active Incident of the logged in user's company.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="paging"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]")]
    public IActionResult GetAllActiveCompanyIncident([FromRoute] string? status, [FromRoute] DataTableAjaxPostModel paging)
    {
        var result = _incidentQuery.GetAllActiveCompanyIncident(status, paging);
        return Ok(result);
    }

    /// <summary>
    /// Get all Incident of the logged in user's company.
    /// </summary>
    /// <param name="userId">Data segregation rules are applied for the provided UserId. If 0, logged in UserId is used.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{UserId}")]
    public IActionResult GetAllCompanyIncident([FromRoute] int userId)
    {
        var result = _incidentQuery.GetAllCompanyIncident(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get all active Incident Type of a given company.
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{CompanyId}")]
    public IActionResult GetCompanyIncidentType([FromRoute] int companyId)
    {
        var result = _incidentQuery.GetCompanyIncidentType(companyId);
        return Ok(result);
    }

    /// <summary>
    /// Get Affected Locations.
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="locationType">IMPACTED/AFFECTED etc.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{CompanyId}/{LocationType}")]
    public IActionResult GetAffectedLocations([FromRoute] int companyId, [FromRoute] string locationType) //TODO: Change LocationType to enum
    {
        var result = _incidentQuery.GetAffectedLocations(companyId, locationType);
        return Ok(result);
    }

    /// <summary>
    /// Get an active incident's incident locations.
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="incidentActivationId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentActivationId}")]
    public IActionResult GetIncidentLocations([FromRoute] int companyId, [FromRoute] int incidentActivationId)
    {
        var result = _incidentQuery.GetIncidentLocations(companyId, incidentActivationId);
        return Ok(result);
    }

    /// <summary>
    /// Get Comms Methods for Incident or Task.
    /// </summary>
    /// <param name="itemID">If Type is set to 'TASK', it'll be used as Active Task ID. If not, it'll be used as the Active Incident ID.</param>
    /// <param name="type">Set to 'TASK' for Tasks, or it'll be regarded as Incident.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{ItemID}/{Type}")]
    public IActionResult GetIncidentComms([FromRoute] int itemID, [FromRoute] string type) //TODO: Change Type to enum
    {
        var result = _incidentQuery.GetIncidentComms(itemID, type);
        return Ok(result);
    }

    /// <summary>
    /// Get company Incident.
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="incidentId"></param>
    /// <param name="userStatus"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentId}/{UserStatus}")]
    public IActionResult GetCompanyIncidentById([FromRoute] int companyId, [FromRoute] int incidentId, [FromRoute] string userStatus) //TODO: Change UserStatus to enum
    {
        var result = _incidentQuery.GetCompanyIncidentById(companyId, incidentId, userStatus);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{ObjectID}/{NoteType}/{AttachmentType}")]
    public async Task<IActionResult> GetIncidentTaskNotes([FromRoute] GetIncidentTaskNotesRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{IncidentActivationId}")]
    public async Task<IActionResult> GetIncidentSOSRequest([FromRoute] GetIncidentSOSRequest request, CancellationToken cancellationToken) //TODO: Change UserStatus to enum
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{ActiveIncidentId}")]
    public async Task<IActionResult> CheckUserSOS([FromRoute] CheckUserSOSRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> UpdateSOS([FromBody] UpdateSOSRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{IncidentActivationId}")]
    public async Task<IActionResult> GetActiveIncidentBasic([FromRoute] GetActiveIncidentBasicRequest request, CancellationToken cancellationToken) //TODO: Change UserStatus to enum
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);

    }
    [HttpGet]
    [Route("[action]/{ActiveIncidentId}")]
    public async Task<IActionResult> GetCallToAction([FromRoute] GetCallToActionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    //[HttpPost]
    //[Route("[action]")]
    //public async Task<IActionResult> AddNotes([FromBody] AddNotesRequest request, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(request, cancellationToken);
    //    return Ok(result);
    //}
    [HttpGet]
    [Route("[action]/{ObjectId}/{AttachmentsType}")]
    public async Task<IActionResult> GetAttachments([FromRoute] GetAttachmentsRequest request, CancellationToken cancellationToken) //TODO: Change UserStatus to enum
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{IncidentActivationId}")]
    public async Task<IActionResult> GetIndidentTimeline([FromRoute] GetIndidentTimelineRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}