using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelPushResponse
{
    public class HandelPushResponseHandler : IRequestHandler<HandelPushResponseRequest, HandelPushResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelPushResponseHandler> _logger;
        public HandelPushResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelPushResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelPushResponse> Handle(HandelPushResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelPushResponseRequest));
            var response = await _communicationQuery.HandelPushResponse(request);
            return response;
        }
    }
}
