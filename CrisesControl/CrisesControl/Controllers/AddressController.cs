using CrisesControl.Api.Application.Commands.Addresses.AddAddress;
using CrisesControl.Api.Application.Commands.Addresses.DeleteAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAllAddress;
using CrisesControl.Api.Application.Commands.Addresses.UpdateAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AddressController(IMediator mediator)
        {
            this._mediator = mediator;          
        }
        [HttpGet]
        [Route("GetAllAddress")]
        public async Task<IActionResult> GetAllAddress([FromRoute] GetAllAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetAddress/{AddressId}")]
        public async Task<IActionResult> GetAddress([FromRoute] GetAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete]
        [Route("DeleteAddress/{AddressId}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] DeleteAddressRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
