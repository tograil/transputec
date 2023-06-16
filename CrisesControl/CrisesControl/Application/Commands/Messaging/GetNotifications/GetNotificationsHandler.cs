using Ardalis.GuardClauses;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetNotifications
{
    public class GetNotificationsHandler: IRequestHandler<GetNotificationsRequest, GetNotificationsResponse>
    {
        private readonly IMessageRepository _messageRepository;
        public GetNotificationsHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<GetNotificationsResponse> Handle(GetNotificationsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetNotificationsRequest));
            var response = new GetNotificationsResponse();
            response.Data =  await _messageRepository.MessageNotifications(request.CompanyId,request.CurrentUserId);
            return response;
        }
    }
}
