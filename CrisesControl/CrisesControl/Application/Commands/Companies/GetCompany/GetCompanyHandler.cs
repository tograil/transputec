using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompany {
    public class GetCompanyHandler : IRequestHandler<GetCompanyRequest, GetCompanyResponse> {
        private readonly GetCompanyValidator _companyValidator;
        private readonly ICompanyQuery _companyQuery;

        public GetCompanyHandler(GetCompanyValidator companyValidator, ICompanyQuery companyQuery) {
            _companyValidator = companyValidator;
            _companyQuery = companyQuery;
        }

        public async Task<GetCompanyResponse> Handle(GetCompanyRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetCompanyRequest));

            await _companyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.GetCompany(request, cancellationToken);

            return companyInfo;
        }
    }
}


