using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.StartConference
{
    public class StartConferenceHandler : IRequestHandler<StartConferenceRequest, StartConferenceResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageQuery _messageQuery;
        public StartConferenceHandler(IMapper mapper, IMessageQuery messageRepository)
        {
            _mapper = mapper;
            _messageQuery = messageRepository;
        }

        public async Task<StartConferenceResponse> Handle(StartConferenceRequest request, CancellationToken cancellationToken)
        {
           
            var result = await _messageQuery.StartConference(request);
            return result;
        }
    }
}
