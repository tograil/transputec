using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ReplyToMessage
{
    public class ReplyToMessageHandler : IRequestHandler<ReplyToMessageRequest, ReplyToMessageResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public ReplyToMessageHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ReplyToMessageResponse> Handle(ReplyToMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReplyToMessageRequest));
            var response = new ReplyToMessageResponse();
            return response;
        }
    }
}
