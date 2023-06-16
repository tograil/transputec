using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioText
{
    public class TwilioTextHandler : IRequestHandler<TwilioTextRequest, TwilioTextResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioTextHandler> _logger;
        public TwilioTextHandler(ICommunicationQuery communicationQuery, ILogger<TwilioTextHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioTextResponse> Handle(TwilioTextRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioTextRequest));
            var response = await _communicationQuery.TwilioText(request);
            return response;
        }
    }
}
