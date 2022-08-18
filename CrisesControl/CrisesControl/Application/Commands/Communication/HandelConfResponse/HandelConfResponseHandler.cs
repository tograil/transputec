using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelConfResponse
{
    public class HandelConfResponseHandler : IRequestHandler<HandelConfResponseRequest, HandelConfResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelConfResponseHandler> _logger;
        public HandelConfResponseHandler(ICommunicationQuery communicationQuery, ILogger<HandelConfResponseHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelConfResponse> Handle(HandelConfResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelConfResponseRequest));
            var response = await _communicationQuery.HandelConfResponse(request);
            return response;
        }
    }
}
