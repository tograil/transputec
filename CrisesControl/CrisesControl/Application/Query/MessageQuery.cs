using AutoMapper;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetMessages;
using CrisesControl.Api.Application.Commands.Messaging.GetNotifications;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using CrisesControl.Api.Application.Commands.Messaging.GetPingInfo;
using CrisesControl.Api.Application.Commands.Messaging.GetReplies;
using CrisesControl.Api.Application.Commands.Messaging.StartConference;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query {
    public class MessageQuery : IMessageQuery {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageQuery> _logger;
        private readonly ICurrentUser _currentUser;

        public MessageQuery(IMessageRepository messageRepository, IMapper mapper, ILogger<MessageQuery> logger, ICurrentUser currentUser) {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request) {
            var countresult = await _messageRepository.GetNotificationsCount(request.CurrentUserId);
            GetNotificationsCountResponse result = _mapper.Map<UserMessageCount, GetNotificationsCountResponse>(countresult);
            return result;
        }

        public async Task<GetNotificationsResponse> GetNotifications(GetNotificationsRequest request) {
            var countresult = await _messageRepository.MessageNotifications(_currentUser.CompanyId, _currentUser.UserId);
            var result = _mapper.Map<NotificationDetails>(countresult);
            var response = new GetNotificationsResponse();
            response.Data = result;
            return response;
        }

        public async Task<GetMessageResponseResponse> GetMessageResponse(GetMessageResponseRequest request) {
            var msgresponse = await _messageRepository.GetMessageResponse(request.ResponseID, request.MessageType);
            var response = _mapper.Map<CompanyMessageResponse>(msgresponse);
            var result = new GetMessageResponseResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetMessageResponsesResponse> GetMessageResponses(GetMessageResponsesRequest request, CancellationToken token) {
            var msgresponse = await _messageRepository.GetMessageResponses(request.MessageType, request.Status);

            if (msgresponse.Count <= 0) {
                _messageRepository.CopyMessageResponse(_currentUser.CompanyId, _currentUser.UserId, _currentUser.TimeZone, token);
                msgresponse = await _messageRepository.GetMessageResponses(request.MessageType, request.Status);
            }

            var response = _mapper.Map<List<CompanyMessageResponse>>(msgresponse);
            var result = new GetMessageResponsesResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request) {
            var msgresponse = await _messageRepository.GetMessages(request.TargetUserId, request.MessageType, request.IncidentActivationId);
            var response = _mapper.Map<List<UserMessageList>>(msgresponse);
            var result = new GetMessagesResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetMessageDetailsResponse> GetMessageDetails(GetMessageDetailsRequest request) {
            var msgresponse = await _messageRepository.GetMessageDetails(request.CloudMsgId, request.MessageId);
            var response = _mapper.Map<IncidentMessageDetails>(msgresponse);
            var result = new GetMessageDetailsResponse();
            result.Data = response;
            result.ErrorCode = System.Net.HttpStatusCode.OK;
            return result;
        }

        public async Task<GetRepliesResponse> GetReplies(GetRepliesRequest request) {
            var replies = await _messageRepository.GetReplies(request.ParentId);
            var response = _mapper.Map<List<MessageDetails>>(replies);
            var result = new GetRepliesResponse();
            result.Data = response;
            result.ErrorCode = System.Net.HttpStatusCode.OK;
            return result;
        }

        public async Task<GetMessageGroupListResponse> GetMessageGroupList(GetMessageGroupListRequest request) {
            var msgresponse = await _messageRepository.GetMessageGroupList(request.MessageID);
            var response = _mapper.Map<List<MessageGroupObject>>(msgresponse);
            var result = new GetMessageGroupListResponse();
            result.data = response;
            result.Message = "Data Loaded Successfully";
            return result;
        }

        public async Task<StartConferenceResponse> StartConference(StartConferenceRequest request) {
            var conference = await _messageRepository.StartConference(request.UserList, request.ObjectID, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone); ;
            var result = _mapper.Map<bool>(conference);
            var response = new StartConferenceResponse();
            if (result != null) {
                response.Status = result;
                response.Message = "Conference service is enabled";
            } else {
                response.Status = false;
                response.Message = "Conference service not enabled";
            }
            return response;
        }

        public async Task<GetPingInfoResponse> GetPingInfo(GetPingInfoRequest request) {
            var result = await _messageRepository.GetPingInfo(request.MessageId, _currentUser.UserId, _currentUser.CompanyId);

            var response = new GetPingInfoResponse();
            response.Data = result;
            response.ErrorCode = System.Net.HttpStatusCode.OK;
            return response;
        }
    }
}
