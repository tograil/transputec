using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using MediatR;
using Serilog;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged
{
    public class MessageAcknowledgeHandler : IRequestHandler<MessageAcknowledgeRequest, MessageAcknowledgeResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageAcknowledgeHandler> _logger;
        private readonly IMapper _mapper;
        public MessageAcknowledgeHandler(IMessageRepository messageRepository,ILogger<MessageAcknowledgeHandler> logger, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _currentUser = currentUser; 
        }
        public async Task<MessageAcknowledgeResponse> Handle(MessageAcknowledgeRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var msgResponse = await _messageRepository.MessageAcknowledged(request.CompanyId, request.MsgListId, _currentUser.TimeZone, request.UserLocationLat, request.UserLocationLong, _currentUser.UserId, request.ResponseID, request.AckMethod);
                var response = _mapper.Map<MessageAckDetails>(msgResponse);
                if (response != null)
                {
                    var Msglist =await _messageRepository.MessageNotifications(request.CompanyId, _currentUser.UserId);
                    return new MessageAcknowledgeResponse
                    {
                        MessageAckDetails = response,
                        MessageListData=Msglist,
                        MessageListId = response.MessageListId,
                        ErrorCode =HttpStatusCode.OK,
                        Message = "Data has been loaded Succesfully"
                    };
                }
                return new MessageAcknowledgeResponse
                {
                    MessageListId = response.MessageListId,
                    ErrorCode =HttpStatusCode.NotFound,
                    Message = "Data Not Found"
                };



            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return null;
            }
        }
    }
}
