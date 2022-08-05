using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.StartConference
{
    public class StartConferenceHandler : IRequestHandler<StartConferenceRequest, StartConferenceResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public StartConferenceHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<StartConferenceResponse> Handle(StartConferenceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(StartConferenceRequest));
            var response = new StartConferenceResponse();
            return response;
        }
    }
}
