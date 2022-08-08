using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.RejoinConference
{
    public class RejoinConferenceHandler : IRequestHandler<RejoinConferenceRequest, RejoinConferenceResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public RejoinConferenceHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<RejoinConferenceResponse> Handle(RejoinConferenceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(RejoinConferenceRequest));
            var response = new RejoinConferenceResponse();
            return response;
        }
    }
}
