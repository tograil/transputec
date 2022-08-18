using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCallAck
{
    public class TwilioCallAckHandler : IRequestHandler<TwilioCallAckRequest, TwilioCallAckResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<TwilioCallAckHandler> _logger;
        public TwilioCallAckHandler(ICommunicationQuery communicationQuery, ILogger<TwilioCallAckHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<TwilioCallAckResponse> Handle(TwilioCallAckRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TwilioCallAckRequest));
            var response = await _communicationQuery.TwilioCallAck(request);
            return response;
        }
    }
}
