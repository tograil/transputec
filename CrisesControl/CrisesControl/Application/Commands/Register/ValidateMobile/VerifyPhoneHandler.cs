using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ValidateMobile
{
    public class VerifyPhoneHandler : IRequestHandler<VerifyPhoneRequest, VerifyPhoneResponse>
    {
        private readonly ILogger<VerifyPhoneHandler> _logger;
        public readonly IRegisterQuery _registerQuery;
        private readonly VerifyPhoneValidator _verifyValidator;
        public VerifyPhoneHandler(ILogger<VerifyPhoneHandler> logger, IRegisterQuery registerQuery, VerifyPhoneValidator verifyValidator)
        {
            this._logger = logger;
            this._registerQuery = registerQuery;
            this._verifyValidator=verifyValidator;
        }
        public async Task<VerifyPhoneResponse> Handle(VerifyPhoneRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(VerifyPhoneRequest));

            await _verifyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var phone = await _registerQuery.ValidateMobile(request);

            return phone;
        }
    }
}
