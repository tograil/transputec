using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioEndConferenceCall
{
    public class TwilioEndConferenceCallHandler : IRequestHandler<TwilioEndConferenceCallRequest, TwilioEndConferenceCallResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioEndConferenceCallHandler> _logger;
        public TwilioEndConferenceCallHandler(ICommunicationQuery communicationQuery, ILogger<TwilioEndConferenceCallHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioEndConferenceCallResponse> Handle(TwilioEndConferenceCallRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioEndConferenceCallRequest));
            var response = await _communicationQuery.TwilioEndConferenceCall(request);
            return response;
        }
    }
}
