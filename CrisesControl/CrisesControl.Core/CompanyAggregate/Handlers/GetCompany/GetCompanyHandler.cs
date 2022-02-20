using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CrisesControl.Core.CompanyAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany
{
    public class GetCompanyHandler : IRequestHandler<GetCompanyRequest, GetCompanyResponse>
    {
        private readonly GetCompanyValidator _companyValidator;
        private readonly ICompanyRepository _companyService;

        public GetCompanyHandler(GetCompanyValidator companyValidator, ICompanyRepository companyService)
        {
            _companyValidator = companyValidator;
            _companyService = companyService;
        }

        public async Task<GetCompanyResponse> Handle(GetCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyRequest));

            await _companyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companies = await _companyService.GetAllCompanies();

            return new GetCompanyResponse();
        }
    }
}