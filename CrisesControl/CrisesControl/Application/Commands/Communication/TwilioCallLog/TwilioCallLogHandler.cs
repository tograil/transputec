using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCallLog
{
    public class TwilioCallLogHandler : IRequestHandler<TwilioCallLogRequest, TwilioCallLogResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioCallLogHandler> _logger;
        public TwilioCallLogHandler(ICommunicationQuery communicationQuery, ILogger<TwilioCallLogHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioCallLogResponse> Handle(TwilioCallLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioCallLogRequest));
            var response = await _communicationQuery.TwilioCallLog(request);
            return response;
        }
    }
}
