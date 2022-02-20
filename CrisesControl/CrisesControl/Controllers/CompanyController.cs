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
}