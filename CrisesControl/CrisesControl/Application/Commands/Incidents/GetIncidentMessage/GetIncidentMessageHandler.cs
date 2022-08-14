using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentMessage
{
    public class GetIncidentMessageHandler : IRequestHandler<GetIncidentMessageRequest, GetIncidentMessageResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentMessageHandler> _logger;
        private readonly GetIncidentMessageValidator _getIncidentMessageValidator;
        public GetIncidentMessageHandler(ILogger<GetIncidentMessageHandler> logger, IIncidentQuery incidentQuery, GetIncidentMessageValidator getIncidentMessageValidator)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
            _getIncidentMessageValidator = getIncidentMessageValidator;
        }
        public async Task<GetIncidentMessageResponse> Handle(GetIncidentMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentMessageRequest));
            await _getIncidentMessageValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetIncidentMessage(request);
            return result;
        }
    }
}
