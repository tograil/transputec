using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteIncidentAsset
{
    public class DeleteIncidentAssetHandler : IRequestHandler<DeleteIncidentAssetRequest, DeleteIncidentAssetResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<DeleteIncidentAssetHandler> _logger;
        private readonly DeleteIncidentAssetValidator _deleteIncidentAssetValidator;
        public DeleteIncidentAssetHandler(IIncidentQuery incidentQuery, ILogger<DeleteIncidentAssetHandler> logger, DeleteIncidentAssetValidator deleteCompanyIncidentValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._deleteIncidentAssetValidator = deleteCompanyIncidentValidator;
        }
        public async Task<DeleteIncidentAssetResponse> Handle(DeleteIncidentAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteIncidentAssetRequest));
            await _deleteIncidentAssetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.DeleteIncidentAsset(request);
            return result;
        }
    }
}
