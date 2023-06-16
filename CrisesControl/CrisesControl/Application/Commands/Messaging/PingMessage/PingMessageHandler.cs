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

            bool hasRecipient = await _messageRepository.HasRecipients(0, request.IncidentActivationId, request.UsersToNotify, request.PingMessageObjLst, "ALL", request.SendToAllRecipient);

            var response = new PingMessageResponse();

            if (hasRecipient) {
    
                //var mappedRequest = _mapper.Map<PingMessageQuery>(request);
                PingMessageQuery pingMessage = new PingMessageQuery();
                pingMessage.CompanyId = _currentUser.CompanyId;
                pingMessage.MessageText = request.MessageText;
                pingMessage.AckOptions = request.AckOptions;
                pingMessage.Priority = request.Priority;
                pingMessage.MultiResponse = request.MultiResponse;
                pingMessage.MessageType = request.MessageType;
                pingMessage.IncidentActivationId = request.IncidentActivationId;
                pingMessage.CurrentUserId = _currentUser.UserId;
                pingMessage.TimeZoneId = _currentUser.TimeZone;
                pingMessage.PingMessageObjLst = request.PingMessageObjLst;
                pingMessage.UsersToNotify = request.UsersToNotify;
                pingMessage.AudioAssetId = request.AudioAssetId;
                pingMessage.SilentMessage = request.SilentMessage;
                pingMessage.MessageMethod = request.MessageMethod;
                pingMessage.MediaAttachments = request.MediaAttachments;
                pingMessage.CascadePlanID = request.CascadePlanID;
                pingMessage.SocialHandle = request.SocialHandle;
                pingMessage.CascadePlanID = request.CascadePlanID;
                pingMessage.SendToAllRecipient = request.SendToAllRecipient;

                var result = await _messageRepository.PingMessages(pingMessage);

                bool isFundAvailable = _messageRepository.IsFundAvailable();
                response.IsFundAvailable = isFundAvailable;
                response.MessageId = result;
                response.Message = "success";
            } else {
                response.MessageId = 0;
                response.Message = "error";
                response.ErrorId = 236;
            }
            
            return response;
        }
    }
}
