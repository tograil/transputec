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
        public ReplyToMessageHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<ReplyToMessageResponse> Handle(ReplyToMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReplyToMessageRequest));
            var response = new ReplyToMessageResponse();
            response = await _messageRepository.ReplyToMessage(request.ParentID, request.MessageText, request.ReplyTo, request.MessageType, request.ActiveIncidentID, request.MessageMethod, request.CascadePlanID, request.CurrentUserId, request.CompanyId, request.TimeZoneId);
            return response;
        }
    }
}
