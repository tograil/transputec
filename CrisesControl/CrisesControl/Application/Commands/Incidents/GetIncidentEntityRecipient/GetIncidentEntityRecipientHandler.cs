using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentEntityRecipient
{

    public class GetIncidentEntityRecipientHandler : IRequestHandler<GetIncidentEntityRecipientRequest, GetIncidentEntityRecipientResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentEntityRecipientHandler> _logger;
        //private readonly GetIncidentRecipientEntityValidator _getIncidentRecipientEntityValidator;
        public GetIncidentEntityRecipientHandler(ILogger<GetIncidentEntityRecipientHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
           // _getIncidentRecipientEntityValidator = getIncidentRecipientEntityValidator;
        }
        public async Task<GetIncidentEntityRecipientResponse> Handle(GetIncidentEntityRecipientRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentEntityRecipientRequest));
            //await _getIncidentRecipientEntityValidator.ValidateAndThrowAsync(request, cancellationToken);
            //var result = await _incidentQuery.GetIncidentEntityRecipient(request);
            return null;
        }
    }
}
