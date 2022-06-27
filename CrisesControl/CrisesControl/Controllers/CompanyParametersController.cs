﻿using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveParameter;
using CrisesControl.Api.Application.Commands.CompanyParameters.UpdateCompanyParameters;
using CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.SavePriority;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyParameterByName;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyParametersController : ControllerBase {
        private readonly IMediator _mediator;
        private readonly ICompanyParametersQuery _companyParametersQuery;

        public CompanyParametersController(IMediator mediator, ICompanyParametersQuery companyParametersQuery) {
            this._mediator = mediator;
            this._companyParametersQuery = companyParametersQuery;
        }
        [HttpGet]
        [Route("GetCascading/{CompanyID:int}/{PlanID:int}")]
        public async Task<IActionResult> GetCascading([FromRoute] GetCascadingRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetCompanyFTP/{CompanyId:int}")]
        public async Task<IActionResult> GetCompanyFTP([FromRoute] GetCompanyFTPRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get all the company parameters by company id.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllCompanyParameters/{CompanyId:int}")]
        public async Task<IActionResult> GetAllCompanyParameters([FromRoute] GetAllCompanyParametersRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Save company FTP.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveCompanyFTP")]
        public async Task<IActionResult> SaveCompanyFTP([FromBody] SaveCompanyFTPRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("SaveCascading")]
        public async Task<IActionResult> SaveCascading([FromBody] SaveCascadingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("SaveParameter")]
        public async Task<IActionResult> SaveParameter([FromBody] SaveParameterRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateCompanyParameters")]
        public async Task<IActionResult> UpdateCompanyParameters([FromBody] UpdateCompanyParametersRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpDelete]
        [Route("DeleteCascading/{PlanID}/{CompanyId}")]
        public async Task<IActionResult> DeleteCascading([FromRoute] DeleteCascadingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("SavePriority")]
        public async Task<IActionResult> SavePriority([FromBody] SavePriorityRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("GetCompanyParameterByName/{CustomerId}/{ParamName}")]
        public async Task<IActionResult> GetCompanyParameterByName([FromRoute] GetCompanyParameterByNameRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
