using Ardalis.GuardClauses;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ConvertToMp3
{
    public class ConvertToMp3Handler:IRequestHandler<ConvertToMp3Request,ConvertToMp3Response>
    {
        private readonly IMessageRepository _messageRepository;
        public ConvertToMp3Handler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<ConvertToMp3Response> Handle(ConvertToMp3Request request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ConvertToMp3Request));

            var response = new ConvertToMp3Response();
            _messageRepository.AcknowledgeMessage
            return response;
        }
    }
}
