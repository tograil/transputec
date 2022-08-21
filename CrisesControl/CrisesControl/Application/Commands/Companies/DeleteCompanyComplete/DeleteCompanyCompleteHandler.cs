using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompanyComplete
{
    public class DeleteCompanyCompleteHandler : IRequestHandler<DeleteCompanyCompleteRequest, DeleteCompanyCompleteResponse>
    {
        private readonly ILogger<DeleteCompanyCompleteHandler> _logger;
        private readonly ICompanyQuery _companyQuery;
        private readonly DeleteCompanyCompleteValidator _deleteCompanyValidator;
        public DeleteCompanyCompleteHandler(ICompanyQuery companyQuery, ILogger<DeleteCompanyCompleteHandler> logger, DeleteCompanyCompleteValidator deleteCompanyValidator)
        {
            this._companyQuery = companyQuery;
            this._deleteCompanyValidator = deleteCompanyValidator;
            this._logger = logger;
        }
        public async Task<DeleteCompanyCompleteResponse> Handle(DeleteCompanyCompleteRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteCompanyCompleteRequest));

            await _deleteCompanyValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.DeleteCompanyComplete(request);

            return companyInfo;
        }
    }
}
