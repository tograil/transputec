using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.DeleteRecording
{
    public class DeleteRecordingHandler : IRequestHandler<DeleteRecordingRequest, DeleteRecordingResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<DeleteRecordingHandler> _logger;
        public DeleteRecordingHandler(ICommunicationQuery communicationQuery, ILogger<DeleteRecordingHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<DeleteRecordingResponse> Handle(DeleteRecordingRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteRecordingRequest));
            var response = await _communicationQuery.DeleteRecording(request);
            return response;
        }
    }
}
