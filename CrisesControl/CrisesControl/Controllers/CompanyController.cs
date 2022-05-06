using CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CompanyController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICompanyQuery _companyQuery;

    public CompanyController(IMediator mediator,
        ICompanyQuery companyQuery)
    {
        _mediator = mediator;
        _companyQuery = companyQuery;
    }
    [HttpPut]
    [Route("UpdateCompanyDRP")]
    public async Task<IActionResult> UpdateCompanyDRP([FromBody] UpdateCompanyDRPlanRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}