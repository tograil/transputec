using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;

namespace CrisesControl.Api.Application.Query {
    public interface IMessageQuery {
        public Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request);
        public Task<GetMessageResponseResponse> GetMessageResponse(GetMessageResponseRequest request);
        public Task<GetMessageResponsesResponse> GetMessageResponses(GetMessageResponsesRequest request);
        
    }
}
