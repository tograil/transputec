using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.GetTempRegistration
{
    public class GetTempRegistrationHandler : IRequestHandler<GetTempRegistrationRequest, GetTempRegistrationReponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly ILogger<GetTempRegistrationHandler> _logger;
        private readonly GetTempRegistrationValidator _getTempRegistrationValidator;
        public GetTempRegistrationHandler(ILogger<GetTempRegistrationHandler> logger, IRegisterQuery registerQuery, GetTempRegistrationValidator getTempRegistrationValidator)
        {
            this._logger = logger;
            this._registerQuery = registerQuery;
            this._getTempRegistrationValidator= getTempRegistrationValidator;
        }
        public async Task<GetTempRegistrationReponse> Handle(GetTempRegistrationRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetTempRegistrationRequest));

            await _getTempRegistrationValidator.ValidateAndThrowAsync(request, cancellationToken);

            var tempReg = await _registerQuery.GetTempRegistration(request);

            return tempReg;
        }
    }
}
