using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Maintenance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class AdminController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICompanyQuery _companyQuery;

    public AdminController(IMediator mediator,
        ICompanyQuery companyQuery)
    {
        _mediator = mediator;
        _companyQuery = companyQuery;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetAllCompanyList([FromQuery] int? status,
        [FromQuery] string? companyProfile,
        CancellationToken cancellationToken)
    {
        var companies = await _companyQuery.GetCompanyList(status, companyProfile);

        return Ok(companies);
    }
    [HttpGet]
    [Route("GetAllLibIncident")]
    public async Task<IActionResult> GetAllLibIncident([FromRoute] GetAllLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("DumpReport/{ReportID}/{DownloadFile}")]
    public async Task<IActionResult> DumpReport([FromRoute] DumpReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetReport/{ReportId}")]
    public async Task<IActionResult> GetReport([FromRoute] GetReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetLibIncident/{LibIncidentId}")]
    public async Task<IActionResult> GetLibIncident([FromRoute] GetLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("AddLibIncident")]
    public async Task<IActionResult> AddLibIncident([FromBody] AddLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateLibIncident")]
    public async Task<IActionResult> UpdateLibIncident([FromBody] UpdateLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("DeleteLibIncident/{LibIncidentId}")]
    public async Task<IActionResult> DeleteLibIncident([FromRoute] DeleteLibIncidentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetAllLibIncidentType")]
    public async Task<IActionResult> GetAllLibIncidentType([FromRoute] GetAllLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetLibIncidentType/{LibIncidentTypeId}")]
    public async Task<IActionResult> GetLibIncidentType([FromRoute] GetLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetCompanyPackageFeatures")]
    public async Task<IActionResult> GetCompanyPackageFeatures([FromRoute] GetCompanyPackageFeaturesRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPost]
    [Route("AddLibIncidentType")]
    public async Task<IActionResult> AddLibIncidentType([FromBody] AddLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateLibIncidentType")]
    public async Task<IActionResult> UpdateLibIncidentType([FromBody] UpdateLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpDelete]
    [Route("DeleteLibIncidentType/{LibIncidentTypeId}")]
    public async Task<IActionResult> DeleteLibIncidentType([FromRoute] DeleteLibIncidentTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}