using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCall
{
    public class TwilioCallHandler : IRequestHandler<TwilioCallRequest, TwilioCallResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioCallHandler> _logger;
        public TwilioCallHandler(ICommunicationQuery communicationQuery, ILogger<TwilioCallHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioCallResponse> Handle(TwilioCallRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioCallRequest));
            var response = await _communicationQuery.TwilioCall(request);
            return response;
        }
    }
}
