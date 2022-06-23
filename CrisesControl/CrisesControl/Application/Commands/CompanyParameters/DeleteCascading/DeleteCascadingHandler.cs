using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading
{
    public class DeleteCascadingHandler : IRequestHandler<DeleteCascadingRequest, DeleteCascadingResponse>
    {
        private readonly ICompanyParametersQuery _companyParametersQuery;
        private readonly ILogger<DeleteCascadingHandler> _logger;
        private readonly DeleteCascadingValidator _deleteCascadingValidator;
        public DeleteCascadingHandler(ICompanyParametersQuery companyParametersQuery, ILogger<DeleteCascadingHandler> logger, DeleteCascadingValidator deleteCascadingValidator)
        {
            this._companyParametersQuery = companyParametersQuery;
            this._deleteCascadingValidator = deleteCascadingValidator;
            this._logger = logger;
        }
        public async Task<DeleteCascadingResponse> Handle(DeleteCascadingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteCascadingRequest));

            await _deleteCascadingValidator.ValidateAndThrowAsync(request, cancellationToken);

            var cascades = await _companyParametersQuery.DeleteCascading(request);
            return cascades;
        }
    }
}
