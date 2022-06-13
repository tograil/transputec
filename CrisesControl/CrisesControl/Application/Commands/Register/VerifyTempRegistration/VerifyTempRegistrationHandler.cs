using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.VerifyTempRegistration
{
    public class VerifyTempRegistrationHandler:IRequestHandler<VerifyTempRegistrationRequest, VerifyTempRegistrationResponse>
    {
        private readonly ILogger<VerifyTempRegistrationHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly VerifyTempRegistrationValidator _verifyValidator;
        public VerifyTempRegistrationHandler(ILogger<VerifyTempRegistrationHandler> logger, IRegisterQuery registerQuery, VerifyTempRegistrationValidator verifyValidator)
        {
          _registerQuery = registerQuery;
            _logger = logger;
            _verifyValidator = verifyValidator;
        }

        public async Task<VerifyTempRegistrationResponse> Handle(VerifyTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(VerifyTempRegistrationRequest));

             await _verifyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var phone = await _registerQuery.VerifyTempRegistration(request);

            return phone;
        }
    }
}
