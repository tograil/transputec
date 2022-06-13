using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
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
}