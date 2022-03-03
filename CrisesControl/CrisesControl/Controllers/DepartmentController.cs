using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DepartmentModel = CrisesControl.Core.Models.EmptyDepartment;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [AllowAnonymous]
    public class DepartmentController : Controller
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] GetDepartmentRequest companyId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(companyId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartment(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentModel departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentModel departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment([FromBody] DepartmentModel departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }
    }
}
