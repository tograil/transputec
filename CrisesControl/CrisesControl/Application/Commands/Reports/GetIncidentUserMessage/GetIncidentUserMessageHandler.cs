using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage
{
    public class GetIncidentUserMessageHandler : IRequestHandler<GetIncidentUserMessageRequest, GetIncidentUserMessageResponse>
    {
        private readonly ILogger<GetIncidentUserMessageHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIncidentUserMessageValidator _getIncidentUserMessageValidator;
        public GetIncidentUserMessageHandler(ILogger<GetIncidentUserMessageHandler> logger, IReportsQuery reportsQuery, GetIncidentUserMessageValidator getIncidentUserMessageValidator)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
            this._getIncidentUserMessageValidator = getIncidentUserMessageValidator;
        }
        public async Task<GetIncidentUserMessageResponse> Handle(GetIncidentUserMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentUserMessageRequest));
            await _getIncidentUserMessageValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _reportsQuery.GetIncidentUserMessage(request);
            return result;
        }
    }
}
