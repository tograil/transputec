using CrisesControl.Api.Application.Commands.Incidents.AddCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.CloneIncident;
using CrisesControl.Api.Application.Commands.Incidents.CopyIncident;
using CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations;
using CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentById;
using CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentType;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentComms;
using CrisesControl.Api.Application.Commands.Incidents.GetIncidentLocations;
using CrisesControl.Api.Application.Commands.Incidents.InitiateAndLaunchIncident;
using CrisesControl.Api.Application.Commands.Incidents.InitiateCompanyIncident;
using CrisesControl.Api.Application.Commands.Incidents.LaunchCompanyIncident;
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

    public IncidentController(IMediator mediator)
    {
        _mediator = mediator;
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
    public async Task<IActionResult> GetAllActiveCompanyIncident(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllActiveCompanyIncidentRequest()
            {
            },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{UserId}")]
    public async Task<IActionResult> GetAllCompanyIncident(int UserId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllCompanyIncidentRequest() { QUserId = UserId },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}")]
    public async Task<IActionResult> GetCompanyIncidentType(int CompanyId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCompanyIncidentTypeRequest() { CompanyId = CompanyId },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{LocationType}")]
    public async Task<IActionResult> GetAffectedLocations(int CompanyId, string LocationType, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAffectedLocationsRequest() { CompanyId = CompanyId, LocationType = LocationType },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentActivationId}")]
    public async Task<IActionResult> GetIncidentLocations(int CompanyId, int IncidentActivationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetIncidentLocationsRequest() { CompanyId = CompanyId, IncidentActivationId = IncidentActivationId },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{ItemID}/{Type}")]
    public async Task<IActionResult> GetIncidentComms(int ItemID, string Type, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetIncidentCommsRequest() { ItemID = ItemID, Type = Type },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{CompanyId}/{IncidentId}/{UserStatus}")]
    public async Task<IActionResult> GetCompanyIncidentById(int CompanyId, int IncidentId, string UserStatus, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCompanyIncidentByIdRequest() { CompanyId = CompanyId, IncidentId = IncidentId, UserStatus = UserStatus },
            cancellationToken
        );

        return Ok(result);
    }
}