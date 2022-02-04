using CrisesControl.Core.CompanyAggregate.Handlers.GetCompany;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CompanyController : Controller
{
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCompanyRequest(), cancellationToken);

        return Ok(result);
    }
}