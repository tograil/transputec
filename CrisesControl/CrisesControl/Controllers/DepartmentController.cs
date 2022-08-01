using CrisesControl.Api.Application.Commands.Departments;
using CrisesControl.Api.Application.Commands.Departments.CheckDepartment;
using CrisesControl.Api.Application.Commands.Departments.CreateDepartment;
using CrisesControl.Api.Application.Commands.Departments.DeleteDepartment;
using CrisesControl.Api.Application.Commands.Departments.DepartmentStatus;
using CrisesControl.Api.Application.Commands.Departments.GetDepartment;
using CrisesControl.Api.Application.Commands.Departments.GetDepartments;
using CrisesControl.Api.Application.Commands.Departments.SegregationLinks;
using CrisesControl.Api.Application.Commands.Departments.UpdateDepartment;
using CrisesControl.Api.Application.Commands.Departments.UpdateSegregationLink;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DepartmentModel = CrisesControl.Core.Models.EmptyDepartment;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{DepartmentId}")]
        public async Task<IActionResult> CheckDepartment([FromRoute] CheckDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{CompanyId:int}/{DepartmentId:int}")]
        public async Task<IActionResult> GetDepartment([FromRoute] GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentRequest departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateSegregationLink([FromBody] UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{OutUserCompanyId}/{TargetID}/{MemberShipType}/{LinkType}")]
        public async Task<IActionResult> SegregationLinks([FromRoute] SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{OutUserCompanyId}")]
        public async Task<IActionResult> DepartmentStatus([FromRoute] DepartmentStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment([FromQuery] DeleteDepartmentRequest departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }
    }
}
