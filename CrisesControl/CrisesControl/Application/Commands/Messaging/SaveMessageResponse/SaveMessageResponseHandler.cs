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
        public SaveMessageResponseHandler(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<SaveMessageResponseResponse> Handle(SaveMessageResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveMessageResponseRequest));
            var response = new SaveMessageResponseResponse();
            response.ResponseId = await _messageRepository.SaveMessageResponse(request.ResponseId, request.ResponseLabel, request.Description, request.IsSafetyResponse, request.SafetyAckAction, request.MessageType, request.Status, request.CurrentUserId, request.CompanyId, request.TimeZoneId);
            return response;
        }
    }
}
