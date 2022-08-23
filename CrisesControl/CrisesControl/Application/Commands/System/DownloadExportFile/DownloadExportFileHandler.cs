using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.System.DownloadExportFile
{
    public class DownloadExportFileHandler : IRequestHandler<DownloadExportFileRequest, DownloadExportFileResponse>
    {
        private readonly ISystemQuery _systemQuery;
        private readonly ILogger<DownloadExportFileHandler> _logger;

        public DownloadExportFileHandler(ISystemQuery systemQuery, ILogger<DownloadExportFileHandler> logger)
        {
            this._logger = logger;
            this._systemQuery = systemQuery;

        }
        public async Task<DownloadExportFileResponse> Handle(DownloadExportFileRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DownloadExportFileRequest));
            var result = await _systemQuery.DownloadExportFile(request);
            return result;
        }
    }
}
