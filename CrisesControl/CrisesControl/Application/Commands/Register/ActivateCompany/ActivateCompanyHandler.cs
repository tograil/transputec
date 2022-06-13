using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.ActivateCompany
{
    public class ActivateCompanyHandler : IRequestHandler<ActivateCompanyRequest, ActivateCompanyResponse>
    {
        private readonly IRegisterQuery _registerQuery;
        private readonly ActivateCompanyValidator _activateCompanyValidator;
        public ActivateCompanyHandler(IRegisterQuery registerQuery, ActivateCompanyValidator activateCompanyValidator)
        {
            _registerQuery = registerQuery;
            _activateCompanyValidator = activateCompanyValidator;
        }
        public async Task<ActivateCompanyResponse> Handle(ActivateCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ActivateCompanyRequest));
            await _activateCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _registerQuery.ActivateCompany(request);
            return result;
        }
    }
}
