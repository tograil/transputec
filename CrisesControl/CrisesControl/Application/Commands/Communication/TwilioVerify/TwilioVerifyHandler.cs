using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerify
{
    public class TwilioVerifyHandler : IRequestHandler<TwilioVerifyRequest, TwilioVerifyResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioVerifyHandler> _logger;
        public TwilioVerifyHandler(ICommunicationQuery communicationQuery, ILogger<TwilioVerifyHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioVerifyResponse> Handle(TwilioVerifyRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioVerifyRequest));
            var response = await _communicationQuery.TwilioVerify(request);
            return response;
        }
    }
}
