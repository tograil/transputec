using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ViewErrorLog
{
    public class ViewErrorLogHandler : IRequestHandler<ViewErrorLogRequest, ViewErrorLogResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<ViewErrorLogHandler> _logger;

        public ViewErrorLogHandler(ISystemQuery systemQuery, ILogger<ViewErrorLogHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<ViewErrorLogResponse> Handle(ViewErrorLogRequest request, CancellationToken cancellationToken)
        {
            var logResponse = await _systemQuery.ViewErrorLog(request);
            return logResponse;
        }
    }
}
