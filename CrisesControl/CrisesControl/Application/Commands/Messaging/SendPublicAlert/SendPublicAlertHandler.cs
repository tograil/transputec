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
        public SendPublicAlertHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<SendPublicAlertResponse> Handle(SendPublicAlertRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SendPublicAlertRequest));
            var response = new SendPublicAlertResponse();
            return response;
        }
    }
}
