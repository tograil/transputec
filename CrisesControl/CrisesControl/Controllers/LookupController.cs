using CrisesControl.Api.Application.Commands.Lookup.AssetTypes;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser;
using CrisesControl.Api.Application.Commands.Lookup.GetCountry;
using CrisesControl.Api.Application.Commands.Lookup.GetIcons;
using CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates;
using CrisesControl.Api.Application.Commands.Lookup.GetTempDept;
using CrisesControl.Api.Application.Commands.Lookup.GetTempLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetTempUser;
using CrisesControl.Api.Application.Commands.Lookup.GetTimezone;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[Action]")]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILookupQuery _lookupQuery;
        public LookupController(IMediator mediator, ILookupQuery lookupQuery)
        {
            this._mediator = mediator;
            _lookupQuery = lookupQuery;
        }

        /// <summary>
        /// Get list of available timezones
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTimeZone([FromRoute] GetTimezoneRequest request)
        {
            try
            {

                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Get list of available countries
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCountry([FromRoute] GetCountryRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Get list of import templates by template type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Type}")]
        public async Task<IActionResult> GetImportTemplates([FromRoute] GetImportTemplatesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get list of available icons
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetIcons([FromRoute] GetIconsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get list of all temporary users
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTmpUser([FromRoute] GetAllTmpUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get list of temporary locations
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTmpLoc([FromRoute] GetAllTmpLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get list of temporary departments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTmpDept([FromRoute] GetAllTmpDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get temporary user's detail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{TempUserId}")]
        public async Task<IActionResult> GetTempUser([FromRoute] GetTempUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get temporary location's detail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{TempLocationId}")]
        public async Task<IActionResult> GetTempLoc([FromRoute] GetTempLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get temporary department's details
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{TempDeptId}")]
        public async Task<IActionResult> GetTempDept([FromRoute] GetTempDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get asset types list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AssetTypes([FromRoute] AssetTypesRequest request)
        {
            var result = await _lookupQuery.AssetTypes();
            return Ok(result);
        }
    }
}