using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.PushTwilioLog
{
    public class PushTwilioLogHandler : IRequestHandler<PushTwilioLogRequest, PushTwilioLogResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<PushTwilioLogHandler> _logger;

        public PushTwilioLogHandler(ISystemQuery systemQuery, ILogger<PushTwilioLogHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<PushTwilioLogResponse> Handle(PushTwilioLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(PushTwilioLogRequest));
            var result = await _systemQuery.PushTwilioLog(request);
            return result;
        }
    }
}
