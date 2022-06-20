using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompany
{
    public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyRequest, DeleteCompanyResponse>
    {
        private readonly ILogger<DeleteCompanyHandler> _logger;
        private readonly ICompanyQuery _companyQuery;
        private readonly DeleteCompanyValidator _deleteCompanyValidator;
        public DeleteCompanyHandler(ICompanyQuery companyQuery, ILogger<DeleteCompanyHandler> logger, DeleteCompanyValidator deleteCompanyValidator)
        {
            this._companyQuery = companyQuery;
            this._deleteCompanyValidator = deleteCompanyValidator;
            this._logger = logger;
        }
        public async Task<DeleteCompanyResponse> Handle(DeleteCompanyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteCompanyRequest));

            await _deleteCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.DeleteCompany(request);

            return companyInfo;
        }
    }
}
