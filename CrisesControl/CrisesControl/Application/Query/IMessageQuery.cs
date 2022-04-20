using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;

namespace CrisesControl.Api.Application.Query {
    public interface IMessageQuery {
        Task<GetNotificationsCountResponse> GetNotificationsCount(GetNotificationsCountRequest request);
    }
}
