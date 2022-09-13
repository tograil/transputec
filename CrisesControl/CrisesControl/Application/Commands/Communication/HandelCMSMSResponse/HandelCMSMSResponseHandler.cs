using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelCMSMSResponse
{
    public class HandelCMSMSResponseHandler : IRequestHandler<HandelCMSMSResponseRequest, HandelCMSMSResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelCMSMSResponseHandler> _logger;
        public HandelCMSMSResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelCMSMSResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelCMSMSResponse> Handle(HandelCMSMSResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelCMSMSResponseRequest));
            var response = await _communicationQuery.HandelCMSMSResponse(request);
            return response;
        }
    }
}
