using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters {
    public class GetAllCompanyParametersHandler : IRequestHandler<GetAllCompanyParametersRequest, GetAllCompanyParametersResponse> {
        private readonly ICompanyParametersQuery _companyParamsQuery;
        private readonly GetCompanyParametersValidator _getAllCompanyParametersValidator;

        public GetAllCompanyParametersHandler(ICompanyParametersQuery companyParamsQuery,
            GetCompanyParametersValidator getAllCompanyParametersValidator) {
            _companyParamsQuery = companyParamsQuery;
            _getAllCompanyParametersValidator = getAllCompanyParametersValidator;
        }

        public async Task<GetAllCompanyParametersResponse> Handle(GetAllCompanyParametersRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetAllCompanyParametersRequest));
            await _getAllCompanyParametersValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _companyParamsQuery.GetAllCompanyParameters(request);
            return result;
        }
    }
}
