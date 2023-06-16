using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelUnifonicCallResponse
{
    public class HandelUnifonicCallResponseHandler : IRequestHandler<HandelUnifonicCallResponseRequest, HandelUnifonicCallResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelUnifonicCallResponseHandler> _logger;
        public HandelUnifonicCallResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelUnifonicCallResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelUnifonicCallResponse> Handle(HandelUnifonicCallResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelUnifonicCallResponseRequest));
            var response = await _communicationQuery.HandelUnifonicCallResponse(request);
            return response;
        }
    }
}
