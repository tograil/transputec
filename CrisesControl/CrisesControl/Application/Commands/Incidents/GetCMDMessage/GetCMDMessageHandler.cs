using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCMDMessage
{
    public class GetCMDMessageHandler : IRequestHandler<GetCMDMessageRequest, GetCMDMessageResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetCMDMessageHandler> _logger;
        private readonly GetCMDMessageValidator _getCMDMessageValidator;
        public GetCMDMessageHandler(ILogger<GetCMDMessageHandler> logger, IIncidentQuery incidentQuery, GetCMDMessageValidator getCMDMessageValidator)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
            _getCMDMessageValidator = getCMDMessageValidator;
        }
        public async Task<GetCMDMessageResponse> Handle(GetCMDMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCMDMessageRequest));
            await _getCMDMessageValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _incidentQuery.GetCMDMessage(request);
            return result;
        }
    }
}
