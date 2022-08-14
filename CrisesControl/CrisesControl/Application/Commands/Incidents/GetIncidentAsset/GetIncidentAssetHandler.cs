using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAsset
{
    public class GetIncidentAssetHandler : IRequestHandler<GetIncidentAssetRequest, GetIncidentAssetResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentAssetHandler> _logger;
        private readonly GetIncidentAssetValidator _getIncidentAssetValidator;
        public GetIncidentAssetHandler(IIncidentQuery incidentQuery, ILogger<GetIncidentAssetHandler> logger, GetIncidentAssetValidator getIncidentAssetValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._getIncidentAssetValidator = getIncidentAssetValidator;
        }
        public async Task<GetIncidentAssetResponse> Handle(GetIncidentAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentAssetRequest));
            await _getIncidentAssetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetIncidentAsset(request);
            return result;
        }
    }
}
