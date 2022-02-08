using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CrisesControl.Core.CompanyAggregate.Services;
using FluentValidation;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany
{
    public class GetCompanyHandler : IRequestHandler<GetCompanyRequest, GetCompanyResponse>
    {
        private readonly GetCompanyValidator _companyValidator;
        private readonly ICompanyService _companyService;

        public GetCompanyHandler(GetCompanyValidator companyValidator, ICompanyService companyService)
        {
            _companyValidator = companyValidator;
            _companyService = companyService;
        }

        public async Task<GetCompanyResponse> Handle(GetCompanyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guard.Against.Null(request, nameof(GetCompanyRequest));

                _companyValidator.ValidateAndThrow(request);

                var companies = await _companyService.GetAllCompanies();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new GetCompanyResponse();
        }
    }
}