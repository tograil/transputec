using CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetMessages;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;

namespace CrisesControl.Api.Application.Query {
    public interface IMessageQuery {
        public Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request);
        public Task<GetMessageResponseResponse> GetMessageResponse(GetMessageResponseRequest request);
        public Task<GetMessageResponsesResponse> GetMessageResponses(GetMessageResponsesRequest request);
        public Task<GetMessagesResponse> GetMessages(GetMessagesRequest request);
        public Task<GetMessageDetailsResponse> GetMessageDetails(GetMessageDetailsRequest request);
        Task<GetMessageGroupListResponse> GetMessageGroupList(GetMessageGroupListRequest request);
    }
}
