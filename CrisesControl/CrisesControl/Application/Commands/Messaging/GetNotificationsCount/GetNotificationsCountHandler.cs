using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount {
    public class GetNotificationsCountHandler : IRequestHandler<GetNotificationsCountRequest, GetNotificationsCountResponse> {
        private readonly IMessageQuery _messageQuery;
        public GetNotificationsCountHandler(IMessageQuery messageQuery) {
            _messageQuery = messageQuery;
        }
        public async Task<GetNotificationsCountResponse> Handle(GetNotificationsCountRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetNotificationsCountRequest));

            var result = await _messageQuery.GetNotificationsCount(request);

            return result;
        }
    }
}
