using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelSMSResponse
{
    public class HandelSMSResponseHandler : IRequestHandler<HandelSMSResponseRequest, HandelSMSResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelSMSResponseHandler> _logger;
        public HandelSMSResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelSMSResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelSMSResponse> Handle(HandelSMSResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelSMSResponseRequest));
            var response = await _communicationQuery.HandelSMSResponse(request);
            return response;
        }
    }
}
