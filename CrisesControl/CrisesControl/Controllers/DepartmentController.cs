using CrisesControl.Core.DepartmentAggregate.Handles.GetDepartment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DepartmentModel = CrisesControl.Core.Models.Department;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
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

        [HttpPut("departmentId")]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentModel departmentModel, int departmentId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }
    }
}
