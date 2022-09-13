using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerifyCheck
{
    public class TwilioVerifyCheckHandler : IRequestHandler<TwilioVerifyCheckRequest, TwilioVerifyCheckResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioVerifyCheckHandler> _logger;
        public TwilioVerifyCheckHandler(ICommunicationQuery communicationQuery, ILogger<TwilioVerifyCheckHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioVerifyCheckResponse> Handle(TwilioVerifyCheckRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioVerifyCheckRequest));
            var response = await _communicationQuery.TwilioVerifyCheck(request);
            return response;
        }
    }
}
