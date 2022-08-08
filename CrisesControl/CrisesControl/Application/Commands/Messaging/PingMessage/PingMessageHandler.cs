using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageHandler : IRequestHandler<PingMessageRequest, PingMessageResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;

        public PingMessageHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<PingMessageResponse> Handle(PingMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(PingMessageRequest));
            var response = new PingMessageResponse();
            var mappedRequest = _mapper.Map<PingMessageQuery>(request);
            response.PingId = await _messageRepository.PingMessages(mappedRequest);
            return response;
        }
    }
}
