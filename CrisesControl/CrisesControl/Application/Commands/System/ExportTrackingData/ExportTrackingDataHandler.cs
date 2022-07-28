using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ExportTrackingData
{
    public class ExportTrackingDataHandler : IRequestHandler<ExportTrackingDataRequest, ExportTrackingDataResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<ExportTrackingDataHandler> _logger;

        public ExportTrackingDataHandler(ISystemQuery systemQuery, ILogger<ExportTrackingDataHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<ExportTrackingDataResponse> Handle(ExportTrackingDataRequest request, CancellationToken cancellationToken)
        {
            var logResponse = await _systemQuery.ExportTrackingData(request);
            return logResponse;
        }
    }
}
