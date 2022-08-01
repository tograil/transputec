using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.ReactivateCompany
{
    public class ReactivateCompanyHandler : IRequestHandler<ReactivateCompanyRequest, ReactivateCompanyResponse>
    {
        private readonly ReactivateCompanyValidator _reactivateCompanyValidator;
        private readonly ICompanyQuery _companyQuery;
        public ReactivateCompanyHandler(ReactivateCompanyValidator reactivateCompanyValidator, ICompanyQuery companyQuery)
        {
            this._reactivateCompanyValidator = reactivateCompanyValidator;
            this._companyQuery = companyQuery;
        }
        public async Task<ReactivateCompanyResponse> Handle(ReactivateCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReactivateCompanyRequest));

            await _reactivateCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);
            var results = await _companyQuery.ReactivateCompany(request);
            return results;
        }
    }
}
