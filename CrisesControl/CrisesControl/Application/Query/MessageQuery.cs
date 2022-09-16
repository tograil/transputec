﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetMessages;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using CrisesControl.Api.Application.Commands.Messaging.GetReplies;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query {
    public class MessageQuery : IMessageQuery {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageQuery> _logger;

        public MessageQuery(IMessageRepository messageRepository, IMapper mapper, ILogger<MessageQuery> logger) {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request) {
            var countresult = await _messageRepository.GetNotificationsCount(request.CurrentUserId);
            GetNotificationsCountResponse result = _mapper.Map<UserMessageCount, GetNotificationsCountResponse>(countresult);
            return result;
        }

        public async Task<GetMessageResponseResponse> GetMessageResponse(GetMessageResponseRequest request) {
            var msgresponse = await _messageRepository.GetMessageResponse(request.ResponseID, request.MessageType);
            var response = _mapper.Map<CompanyMessageResponse>(msgresponse);
            var result = new GetMessageResponseResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetMessageResponsesResponse> GetMessageResponses(GetMessageResponsesRequest request) {
            var msgresponse = await _messageRepository.GetMessageResponses(request.MessageType, request.Status);
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

        public async Task<GetMessageDetailsResponse> GetMessageDetails(GetMessageDetailsRequest request)
        {
            var msgresponse = await _messageRepository.GetMessageDetails(request.CloudMsgId, request.MessageId);
            var response = _mapper.Map<IncidentMessageDetails>(msgresponse);
            var result = new GetMessageDetailsResponse();
            result.Data = response;
            result.ErrorCode = System.Net.HttpStatusCode.OK;
            return result;
        }

        public async Task<GetRepliesResponse> GetReplies(GetRepliesRequest request)
        {
            var replies = await _messageRepository.GetReplies(request.MessageId);
            var result =  _mapper.Map<List<MessageDetails>>(replies);
            var response = new GetRepliesResponse();
            if (result != null) {
            response.data = result;
                response.Message = "Data loaded";
                     }
            else
            {
                response.data = null;
                response.Message = "No data found";
            }
            return response;
        }

        public async Task<GetMessageGroupListResponse> GetMessageGroupList(GetMessageGroupListRequest request)
        {
            var msgresponse = await _messageRepository.GetMessageGroupList(request.MessageID);
            var response = _mapper.Map<List<MessageGroupObject>>(msgresponse);
            var result = new GetMessageGroupListResponse();
            result.data = response;
            result.Message = "Data Loaded Successfully";
            return result;
        }
    }
}
