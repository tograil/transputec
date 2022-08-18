using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAction
{
    public class GetIncidentActionHandler : IRequestHandler<GetIncidentActionRequest, GetIncidentActionResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentActionHandler> _logger;
        private readonly GetIncidentActionValidator _getIncidentActionValidator;
        public GetIncidentActionHandler(IIncidentQuery incidentQuery, ILogger<GetIncidentActionHandler> logger, GetIncidentActionValidator getIncidentActionValidator)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            this._getIncidentActionValidator = getIncidentActionValidator;
        }
        public async Task<GetIncidentActionResponse> Handle(GetIncidentActionRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentActionRequest));
            await _getIncidentActionValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetIncidentAction(request);
            return result;
        }
    }
}
