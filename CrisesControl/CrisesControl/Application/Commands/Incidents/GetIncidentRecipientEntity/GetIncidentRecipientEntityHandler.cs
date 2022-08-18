using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentRecipientEntity
{
    public class GetIncidentRecipientEntityHandler : IRequestHandler<GetIncidentRecipientEntityRequest, GetIncidentRecipientEntityResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentRecipientEntityHandler> _logger;
        private readonly GetIncidentRecipientEntityValidator _getIncidentRecipientEntityValidator;
        public GetIncidentRecipientEntityHandler(ILogger<GetIncidentRecipientEntityHandler> logger, IIncidentQuery incidentQuery, GetIncidentRecipientEntityValidator getIncidentRecipientEntityValidator)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
            _getIncidentRecipientEntityValidator = getIncidentRecipientEntityValidator;
        }
        public async Task<GetIncidentRecipientEntityResponse> Handle(GetIncidentRecipientEntityRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentRecipientEntityRequest));
            await _getIncidentRecipientEntityValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetIncidentRecipientEntity(request);
            return result;
        }
    }
}
