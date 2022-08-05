using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SaveMessageResponse
{
    public class SaveMessageResponseHandler : IRequestHandler<SaveMessageResponseRequest, SaveMessageResponseResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public SaveMessageResponseHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<SaveMessageResponseResponse> Handle(SaveMessageResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveMessageResponseRequest));
            var response = new SaveMessageResponseResponse();
            return response;
        }
    }
}
