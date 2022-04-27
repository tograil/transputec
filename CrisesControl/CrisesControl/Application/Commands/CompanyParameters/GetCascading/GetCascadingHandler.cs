using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.CompanyParameters.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading
{
    public class GetCascadingHandler : IRequestHandler<GetCascadingRequest, GetCascadingResponse>
    {
        private readonly GetCascadingValidator _cascadingValidator;
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ICompanyParametersRepository _companyParameterService;
        public GetCascadingHandler(GetCascadingValidator cascadingValidator, ICompanyParametersQuery companyParametersQuery, ICompanyParametersRepository companyParameterService)
        {
            this._cascadingValidator = cascadingValidator;
            this._companyParametersQuery = companyParametersQuery;
            this._companyParameterService = companyParameterService;
        }
        public async Task<GetCascadingResponse> Handle(GetCascadingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCascadingRequest));

            await _cascadingValidator.ValidateAndThrowAsync(request, cancellationToken);

            var cascades = await _companyParametersQuery.GetCascading(request);
            return cascades;
        }
    }
}
