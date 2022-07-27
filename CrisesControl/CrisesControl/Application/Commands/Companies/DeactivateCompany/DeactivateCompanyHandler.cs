using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeactivateCompany
{
    public class DeactivateCompanyHandler : IRequestHandler<DeactivateCompanyRequest, DeactivateCompanyResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<DeactivateCompanyHandler> _logger;
        private readonly DeactivateCompanyValidator _deactivateCompanyValidator;
        public DeactivateCompanyHandler(ICompanyQuery companyQuery, ILogger<DeactivateCompanyHandler> logger, DeactivateCompanyValidator deactivateCompanyValidator)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
            this._deactivateCompanyValidator = deactivateCompanyValidator;
        }
        public async Task<DeactivateCompanyResponse> Handle(DeactivateCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeactivateCompanyRequest));

            await _deactivateCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);
            var results = await _companyQuery.DeactivateCompany(request);
            return results;
        }
    }
}
