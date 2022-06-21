using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.ViewCompany
{
    public class ViewCompanyHandler : IRequestHandler<ViewCompanyRequest, ViewCompanyResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ViewCompanyValidator _viewCompanyValidator;
        public ViewCompanyHandler(ICompanyQuery companyQuery, ViewCompanyValidator viewCompanyValidator)
        {
            _companyQuery = companyQuery;
            _viewCompanyValidator = viewCompanyValidator;
        }
        public async Task<ViewCompanyResponse> Handle(ViewCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ViewCompanyRequest));

            await _viewCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.ViewCompany(request);

            return companyInfo;
        }
    }
}
