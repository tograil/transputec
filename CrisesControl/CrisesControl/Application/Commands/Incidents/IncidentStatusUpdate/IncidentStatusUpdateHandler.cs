using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.IncidentStatusUpdate
{
    public class IncidentStatusUpdateHandler : IRequestHandler<IncidentStatusUpdateRequest, IncidentStatusUpdateResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<IncidentStatusUpdateHandler> _logger;
        private readonly IncidentStatusUpdateValidator _incidentStatusUpdateValidator;
        public IncidentStatusUpdateHandler(IIncidentQuery incidentQuery, ILogger<IncidentStatusUpdateHandler> logger, IncidentStatusUpdateValidator incidentStatusUpdateValidator)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
            this._incidentStatusUpdateValidator = incidentStatusUpdateValidator;
        }
        public async Task<IncidentStatusUpdateResponse> Handle(IncidentStatusUpdateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(IncidentStatusUpdateRequest));
            await _incidentStatusUpdateValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.IncidentStatusUpdate(request);
            return result;
        }
    }
}
