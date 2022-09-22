using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.PingMessage
{
    public class PingMessageHandler : IRequestHandler<PingMessageRequest, PingMessageResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<PingMessageHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public PingMessageHandler(IMapper mapper, IMessageRepository messageRepository, ICurrentUser currentUser, ILogger<PingMessageHandler> logger)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<PingMessageResponse> Handle(PingMessageRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(PingMessageRequest));
            var response = new PingMessageResponse();
            //var mappedRequest = _mapper.Map<PingMessageQuery>(request);
            PingMessageQuery pingMessage = new PingMessageQuery();
            pingMessage.AckOptions = request.AckOptions;
            pingMessage.AssetId = request.AssetId;
            pingMessage.CascadePlanID = request.CascadePlanID;
            pingMessage.CompanyId = _currentUser.CompanyId;
            pingMessage.CurrentUserId = _currentUser.UserId;
            pingMessage.IncidentActivationId = request.IncidentActivationId;
            pingMessage.MediaAttachments = request.MediaAttachments;
            pingMessage.MessageMethod = request.MessageMethod;
            pingMessage.MessageText = request.MessageText;
            pingMessage.MessageType = request.MessageType;
            pingMessage.TimeZoneId = _currentUser.TimeZone;
            pingMessage.SilentMessage = request.SilentMessage;
            pingMessage.SocialHandle = request.SocialHandle;
            pingMessage.PingMessageObjLst = request.PingMessageObjLst;
            pingMessage.Priority = request.Priority;
            pingMessage.UsersToNotify = request.UsersToNotify;
            pingMessage.MultiResponse = request.MultiResponse;
            var result = await _messageRepository.PingMessages(pingMessage);
            response.MessageId = result;
            response.Message = "success";
            return response;
        }
    }
}
