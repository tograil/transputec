using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CompanyController : Controller {
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{CompanyId:int}")]
    public async Task<IActionResult> GetCompany([FromRoute] GetCompanyRequest request, CancellationToken cancellationToken) {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("GetCommsMethod")]
    public async Task<IActionResult> GetCommsMethod([FromRoute] GetCommsMethodRequest request, CancellationToken cancellationToken) {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}