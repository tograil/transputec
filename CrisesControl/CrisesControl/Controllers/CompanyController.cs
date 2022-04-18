using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Query;
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
}