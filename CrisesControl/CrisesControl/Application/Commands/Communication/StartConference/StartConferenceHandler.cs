using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Communication.StartConference
{
    public class StartConferenceHandler : IRequestHandler<StartConferenceRequest, StartConferenceResponse>
    {
        private readonly ICommunicationQuery _communicationQuery;
        private readonly ILogger<StartConferenceHandler> _logger;
        public StartConferenceHandler(ICommunicationQuery communicationQuery, ILogger<StartConferenceHandler> logger)
        {
            _communicationQuery = communicationQuery;
            _logger = logger;
        }
        public async Task<StartConferenceResponse> Handle(StartConferenceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(StartConferenceRequest));
            var response = await _communicationQuery.StartConference(request);
            return response;
        }
    }
}
