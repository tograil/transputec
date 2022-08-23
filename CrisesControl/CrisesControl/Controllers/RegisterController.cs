using CrisesControl.Api.Application.Commands.Companies.CompleteRegistration;
using CrisesControl.Api.Application.Commands.Register;
using CrisesControl.Api.Application.Commands.Register.ActivateCompany;
using CrisesControl.Api.Application.Commands.Register.BusinessSector;
using CrisesControl.Api.Application.Commands.Register.CheckAppDownloaded;
using CrisesControl.Api.Application.Commands.Register.CheckCustomer;
using CrisesControl.Api.Application.Commands.Register.CreateSampleIncident;
using CrisesControl.Api.Application.Commands.Register.DeleteTempRegistration;
using CrisesControl.Api.Application.Commands.Register.GetAllPackagePlan;
using CrisesControl.Api.Application.Commands.Register.GetTempRegistration;
using CrisesControl.Api.Application.Commands.Register.Index;
using CrisesControl.Api.Application.Commands.Register.SendCredentials;
using CrisesControl.Api.Application.Commands.Register.SendVerification;
using CrisesControl.Api.Application.Commands.Register.SetupCompleted;
using CrisesControl.Api.Application.Commands.Register.TempRegister;
using CrisesControl.Api.Application.Commands.Register.UpdateCompanyStatus;
using CrisesControl.Api.Application.Commands.Register.UpgradeRequest;
using CrisesControl.Api.Application.Commands.Register.ValidateMobile;
using CrisesControl.Api.Application.Commands.Register.ValidateUserEmail;
using CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRegisterQuery _registerQuery;
        public RegisterController(IMediator mediator, IRegisterQuery registerQuery)
        {
            _mediator = mediator;
            _registerQuery = registerQuery;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] IndexRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("CheckCustomer/{CustomerId}")]
       
        public async Task<IActionResult> CheckCustomer([FromRoute] CheckCustomerRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("ValidateMobile/{Code}/{ISD}/{MobileNo}")]

        public async Task<IActionResult> ValidateMobile([FromRoute] VerifyPhoneRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("ValidateUserEmail/{CompanyId}/{uniqueId}")]

        public async Task<IActionResult> ValidateUserUserEmail([FromRoute] ValidateUserEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("UpgradeRequest/{CompanyId}")]

        public async Task<IActionResult> UpgradeRequest([FromRoute] UpgradeRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost("VerifyTempRegistration")]

        public async Task<IActionResult> VerifyTempRegistration([FromBody] VerifyTempRegistrationRequest request, CancellationToken cancellationToken)
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
        [HttpPut("SetupCompleted")]
        public async Task<IActionResult> SetupCompleted([FromBody] SetupCompletedRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("GetTempRegistration")]
        public async Task<IActionResult> GetTempRegistration([FromRoute] GetTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpDelete("DeleteTempRegistration")]
        public async Task<IActionResult> DeleteTempRegistration([FromRoute] DeleteTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost("CreateSampleIncident/{CompanyId}")]
        public async Task<IActionResult> CreateSampleIncident([FromRoute] CreateSampleIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPut("ActivateCompany/{UserId}/{ActivationKey}/{SalesSource}")]
        public async Task<IActionResult> ActivateCompany([FromBody] ActivateCompanyRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("CheckAppDownload/{UserId}")]
        public async Task<IActionResult> CheckAppDownload([FromRoute] CheckAppDownloadRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("SendVerification/{UniqueId}")]
        public async Task<IActionResult> SendVerification([FromRoute] SendVerificationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("SendCredentials/{UniqueId}")]
        public async Task<IActionResult> SendVerification([FromRoute] SendCredentialsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet("BusinessSector")]
        public async Task<IActionResult> BusinessSector([FromRoute] BusinessSectorRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetAllPackagePlan")]
        public async Task<IActionResult> GetAllPackagePlan([FromRoute] GetAllPackagePlanRequest request, CancellationToken cancellationToken)
        {
            var result = await _registerQuery.GetAllPackagePlan();
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateCompanyStatus")]
        public async Task<IActionResult> UpdateCompanyStatus([FromBody] UpdateCompanyStatusRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("CompleteRegistration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
