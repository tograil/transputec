using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioTextLog
{
    public class TwilioTextLogHandler : IRequestHandler<TwilioTextLogRequest, TwilioTextLogResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioTextLogHandler> _logger;
        public TwilioTextLogHandler(ICommunicationQuery communicationQuery, ILogger<TwilioTextLogHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioTextLogResponse> Handle(TwilioTextLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioTextLogRequest));
            var response = await _communicationQuery.TwilioTextLog(request);
            return response;
        }
    }
}
