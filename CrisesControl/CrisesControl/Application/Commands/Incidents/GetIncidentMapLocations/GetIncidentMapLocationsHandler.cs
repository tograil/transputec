using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMapLocations
{
    public class GetIncidentMapLocationsHandler : IRequestHandler<GetIncidentMapLocationsRequest, GetIncidentMapLocationsResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentMapLocationsHandler> _logger;
        private readonly GetIncidentMapLocationsValidator _getIncidentMapLocationsValidator;
        public GetIncidentMapLocationsHandler(ILogger<GetIncidentMapLocationsHandler> logger, IIncidentQuery incidentQuery, GetIncidentMapLocationsValidator getIncidentMapLocationsValidator)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
            _getIncidentMapLocationsValidator = getIncidentMapLocationsValidator;
        }
        public async Task<GetIncidentMapLocationsResponse> Handle(GetIncidentMapLocationsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentMapLocationsRequest));
            await _getIncidentMapLocationsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetIncidentMapLocations(request);
            return result;
        }
    }
}
