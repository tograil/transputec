using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelCallResponse
{
    public class HandelCallResponseHandler : IRequestHandler<HandelCallResponseRequest, HandelCallResponseResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelCallResponseHandler> _logger;
        public HandelCallResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelCallResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelCallResponseResponse> Handle(HandelCallResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelCallResponseRequest));
            var response = await _communicationQuery.HandelCallResponse(request);
            return response;
        }
    }
}
