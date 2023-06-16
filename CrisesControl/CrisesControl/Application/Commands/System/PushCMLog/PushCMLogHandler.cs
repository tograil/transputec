using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.PushCMLog
{
    public class PushCMLogHandler : IRequestHandler<PushCMLogRequest, PushCMLogResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<PushCMLogHandler> _logger;

        public PushCMLogHandler(ISystemQuery systemQuery, ILogger<PushCMLogHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<PushCMLogResponse> Handle(PushCMLogRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(PushCMLogRequest));
            var result = await _systemQuery.PushCMLog(request);
            return result;
        }
    }
}
