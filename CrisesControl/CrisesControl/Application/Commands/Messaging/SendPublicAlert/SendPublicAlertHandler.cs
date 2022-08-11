using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SendPublicAlert
{
    public class SendPublicAlertHandler : IRequestHandler<SendPublicAlertRequest, SendPublicAlertResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public SendPublicAlertHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<SendPublicAlertResponse> Handle(SendPublicAlertRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendPublicAlertRequest));
            var response = new SendPublicAlertResponse();
            response = await _messageRepository.SendPublicAlert(request.MessageText, request.MessageMethod, request.SchedulePA, request.ScheduleAt, request.SessionId, request.UserID, request.CompanyID, request.TimeZoneId);
            return response;
        }
    }
}
