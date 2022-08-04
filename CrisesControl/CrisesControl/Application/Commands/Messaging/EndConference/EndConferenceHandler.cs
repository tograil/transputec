using Ardalis.GuardClauses;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.EndConference
{
    public class EndConferenceHandler:IRequestHandler<EndConferenceRequest, EndConferenceResponse>
    {
        private readonly IMessageRepository _messageRepository;
        public EndConferenceHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<EndConferenceResponse> Handle(EndConferenceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(EndConferenceRequest));
            var response = new EndConferenceResponse();
            return response;
        }
    }
}
