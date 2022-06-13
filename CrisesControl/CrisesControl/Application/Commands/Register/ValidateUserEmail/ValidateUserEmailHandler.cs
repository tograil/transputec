using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Register.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ValidateUserEmail
{
    public class ValidateUserEmailHandler : IRequestHandler<ValidateUserEmailRequest, ValidateUserEmailResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly ILogger<ValidateUserEmailHandler> _logger;
        public ValidateUserEmailHandler(IRegisterQuery registerQuery, ILogger<ValidateUserEmailHandler> logger)
        {
            this._logger=logger;
            this._registerQuery = registerQuery;    
        }
        public async Task<ValidateUserEmailResponse> Handle(ValidateUserEmailRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ValidateUserEmailRequest));

           // await _verifyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var phone = await _registerQuery.ValidateUserEmail(request);

            return phone;
        }
    }
}
