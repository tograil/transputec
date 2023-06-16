using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CheckCompany
{
    public class CheckCompanyHandler : IRequestHandler<CheckCompanyRequest, CheckCompanyResponse>
    {
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<CheckCompanyHandler> _logger;
        private readonly CheckCompanyValidator _checkCompanyValidator;
        public CheckCompanyHandler(ICompanyQuery companyQuery, ILogger<CheckCompanyHandler> logger, CheckCompanyValidator checkCompanyValidator)
        {
            this._companyQuery = companyQuery;
            this._logger = logger;
            this._checkCompanyValidator = checkCompanyValidator;
        }
        public async Task<CheckCompanyResponse> Handle(CheckCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckCompanyRequest));

            await _checkCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.CheckCompany(request);

            return companyInfo;
        }
    }
}
