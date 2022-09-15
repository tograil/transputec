using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioRecordingLog
{
    public class TwilioRecordingLogHandler : IRequestHandler<TwilioRecordingLogRequest, TwilioRecordingLogResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioRecordingLogHandler> _logger;
        public TwilioRecordingLogHandler(ICommunicationQuery communicationQuery, ILogger<TwilioRecordingLogHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioRecordingLogResponse> Handle(TwilioRecordingLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioRecordingLogRequest));
            var response = await _communicationQuery.TwilioRecordingLog(request);
            return response;
        }
    }
}
