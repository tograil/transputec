using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioConfLog
{
    public class TwilioConfLogHandler : IRequestHandler<TwilioConfLogRequest, TwilioConfLogResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioConfLogHandler> _logger;
        public TwilioConfLogHandler(ICommunicationQuery communicationQuery, ILogger<TwilioConfLogHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioConfLogResponse> Handle(TwilioConfLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioConfLogRequest));
            var response = await _communicationQuery.TwilioConfLog(request);
            return response;
        }
    }
}
