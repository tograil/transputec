using CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.CloneIncident;
using CrisesControl.Api.Application.Commands.Incidents.CopyIncident;
using CrisesControl.Api.Application.Commands.Incidents.InitiateAndLaunchIncident;
using CrisesControl.Api.Application.Commands.Incidents.InitiateCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.LaunchCompanyIncident;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Query.Common;
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

    [HttpGet]
    [Route("[action]")]
    public IActionResult GetAllActiveCompanyIncident([FromRoute] string? Status, [FromRoute] PagedRequest Paging)
    {
        var result = _incidentQuery.GetAllActiveCompanyIncident(Status, Paging);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{UserId}")]
    public IActionResult GetAllCompanyIncident([FromRoute] int UserId)
    {
        var result = _incidentQuery.GetAllCompanyIncident(UserId);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}")]
    public IActionResult GetCompanyIncidentType([FromRoute] int CompanyId)
    {
        var result = _incidentQuery.GetCompanyIncidentType(CompanyId);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{LocationType}")]
    public IActionResult GetAffectedLocations([FromRoute] int CompanyId, [FromRoute] string LocationType)
    {
        var result = _incidentQuery.GetAffectedLocations(CompanyId, LocationType);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentActivationId}")]
    public IActionResult GetIncidentLocations([FromRoute] int CompanyId, [FromRoute] int IncidentActivationId)
    {
        var result = _incidentQuery.GetIncidentLocations(CompanyId, IncidentActivationId);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{ItemID}/{Type}")]
    public IActionResult GetIncidentComms([FromRoute] int ItemID, [FromRoute] string Type)
    {
        var result = _incidentQuery.GetIncidentComms(ItemID, Type);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentId}/{UserStatus}")]
    public IActionResult GetCompanyIncidentById([FromRoute] int CompanyId, [FromRoute] int IncidentId, [FromRoute] string UserStatus)
    {
        var result = _incidentQuery.GetCompanyIncidentById(CompanyId, IncidentId, UserStatus);
        return Ok(result);
    }
}