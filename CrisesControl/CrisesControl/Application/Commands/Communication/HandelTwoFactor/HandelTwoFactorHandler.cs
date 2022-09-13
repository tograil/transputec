using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.HandelTwoFactor
{
    public class HandelTwoFactorHandler : IRequestHandler<HandelTwoFactorRequest, HandelTwoFactorResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<HandelTwoFactorHandler> _logger;
        public HandelTwoFactorHandler(ICommunicationQuery communicationQuery, ILogger<HandelTwoFactorHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<HandelTwoFactorResponse> Handle(HandelTwoFactorRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(HandelTwoFactorRequest));
            var response = await _communicationQuery.HandelTwoFactor(request);
            return response;
        }
    }
}
