using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.SetupCompleted;
using CrisesControl.Api.Application.Commands.Register.TempRegister;
using CrisesControl.Api.Application.Commands.Register.UpgradeRequest;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RegisterController(IMediator mediator)
        {
          _mediator=mediator;
        }
        [HttpGet("CheckCustomer/{CustomerId}")]
       
        public async Task<IActionResult> CheckCustomer([FromRoute] CheckCustomerRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("ValidateMobile")]

        public async Task<IActionResult> ValidateMobile([FromRoute] VerifyPhoneRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("ValidateUserUserEmail")]

        public async Task<IActionResult> ValidateUserUserEmail([FromRoute] VerifyPhoneRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("UpgradeRequest")]

        public async Task<IActionResult> UpgradeRequest([FromRoute] UpgradeRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("VerifyTempRegistration")]

        public async Task<IActionResult> VerifyTempRegistration([FromRoute] VerifyTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost("TempRegister")]

        public async Task<IActionResult> TempRegister([FromBody] TempRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost("SetupCompleted")]
        public async Task<IActionResult> SetupCompleted([FromBody] SetupCompletedRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
