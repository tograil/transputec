using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;

namespace CrisesControl.Core.CompanyAggregate.Handlers.GetCompany
{
    public class GetCompanyHandler : IRequestHandler<GetCompanyRequest, GetCompanyResponse>
    {
        private readonly GetCompanyValidator _companyValidator;

        public GetCompanyHandler(GetCompanyValidator companyValidator)
        {
            _companyValidator = companyValidator;
        }

        public Task<GetCompanyResponse> Handle(GetCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyRequest));

            _companyValidator.ValidateAndThrow(request);

            return Task.FromResult(new GetCompanyResponse());
        }
    }
}