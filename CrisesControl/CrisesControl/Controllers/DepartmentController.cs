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
        private readonly IDepartmentQuery _departmentQuery;

        public DepartmentController(IMediator mediator, IDepartmentQuery departmentQuery) {
            _mediator = mediator;
            _departmentQuery = departmentQuery;
        }
        /// <summary>
        /// Get all department the list 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("{CompanyId:int}/{FilterVirtual}")]
        public async Task<IActionResult> Index([FromRoute] GetDepartmentsRequest request, CancellationToken cancellationToken)
        {
            var result = await _departmentQuery.GetDepartments(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Check the Department existence
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{DepartmentId}")]
        public async Task<IActionResult> CheckDepartment([FromRoute] CheckDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get a Department by CompanyI and DepartmentId
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("[action]/{CompanyId:int}/{DepartmentId:int}")]
        public async Task<IActionResult> GetDepartment([FromRoute] GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Create a Department to the app
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

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
        /// <summary>
        /// Update the Segregation link in the Department
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]/{SourceId}/{TargetId}/{LinkType}")]
        public async Task<IActionResult> UpdateSegregationLink([FromRoute] UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        ///Get a Segregation links in the Department by CompanyId, TargetId, MembershipType and LinkType
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{OutUserCompanyId}/{TargetID}/{MemberShipType}/{LinkType}")]
        public async Task<IActionResult> SegregationLinks([FromRoute] SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Check the  Department Status
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{OutUserCompanyId}")]
        public async Task<IActionResult> DepartmentStatus([FromRoute] DepartmentStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Delete the Department
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment([FromQuery] DeleteDepartmentRequest departmentModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(departmentModel, cancellationToken);
            return Ok(result);
        }
    }
}
