using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.DownloadRecording
{
    public class DownloadRecordingHandler : IRequestHandler<DownloadRecordingRequest, DownloadRecordingResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<DownloadRecordingHandler> _logger;
        public DownloadRecordingHandler(ICommunicationQuery communicationQuery, ILogger<DownloadRecordingHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<DownloadRecordingResponse> Handle(DownloadRecordingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DownloadRecordingRequest));
            var response = await _communicationQuery.DownloadRecording(request);
            return response;

        }
    }
}
