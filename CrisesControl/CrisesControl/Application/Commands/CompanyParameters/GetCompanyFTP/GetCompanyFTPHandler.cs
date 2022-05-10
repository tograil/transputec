using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.CompanyParameters.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP
{
    public class GetCompanyFTPHandler : IRequestHandler<GetCompanyFTPRequest, GetCompanyFTPResponse>
    {
        private readonly GetCompanyFTPValidator _companyFTPValidator;
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ICompanyParametersRepository _companyParameterRepository;
        public GetCompanyFTPHandler(GetCompanyFTPValidator companyFTPValidator, ICompanyParametersQuery companyParametersQuery, ICompanyParametersRepository companyParameterService)
        {
            this._companyParametersQuery = companyParametersQuery;
            this._companyFTPValidator=companyFTPValidator;
            this._companyParameterRepository = companyParameterService;

        }
        public async Task<GetCompanyFTPResponse> Handle(GetCompanyFTPRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyFTPRequest));

            await _companyFTPValidator.ValidateAndThrowAsync(request, cancellationToken);

            var cascades = await _companyParametersQuery.GetCompanyFTP(request);
            return cascades;
        }
    }
}
