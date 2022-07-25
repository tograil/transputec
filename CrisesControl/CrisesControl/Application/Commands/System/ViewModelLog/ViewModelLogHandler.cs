using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ViewModelLog
{
    public class ViewModelLogHandler : IRequestHandler<ViewModelLogRequest, ViewModelLogResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<ViewModelLogHandler> _logger;

        public ViewModelLogHandler(ISystemQuery systemQuery, ILogger<ViewModelLogHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<ViewModelLogResponse> Handle(ViewModelLogRequest request, CancellationToken cancellationToken)
        {
            var logResponse = await _systemQuery.ViewModelLog(request);
            return logResponse;
        }
    }
}
