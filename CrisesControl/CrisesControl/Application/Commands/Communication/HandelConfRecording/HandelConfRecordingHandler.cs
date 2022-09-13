using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelConfRecording
{
    public class HandelConfRecordingHandler : IRequestHandler<HandelConfRecordingRequest, HandelConfRecordingResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelConfRecordingHandler> _logger;
        public HandelConfRecordingHandler(ICommunicationQuery communicationQuery, ILogger<HandelConfRecordingHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelConfRecordingResponse> Handle(HandelConfRecordingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelConfRecordingRequest));
            var response = await _communicationQuery.HandelConfRecording(request);
            return response;
        }
    }
}
