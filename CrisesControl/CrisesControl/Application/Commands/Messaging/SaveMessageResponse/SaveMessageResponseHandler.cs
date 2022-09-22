using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SaveMessageResponse
{
    public class SaveMessageResponseHandler : IRequestHandler<SaveMessageResponseRequest, SaveMessageResponseResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly ICurrentUser _currentUser;
        public SaveMessageResponseHandler(IMapper mapper, IMessageRepository messageRepository, ICurrentUser currentUser)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _currentUser = currentUser;
        }

        public async Task<SaveMessageResponseResponse> Handle(SaveMessageResponseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveMessageResponseRequest));
            var response = new SaveMessageResponseResponse();
            response.ResponseId = await _messageRepository.SaveMessageResponse(request.ResponseId, request.ResponseLabel, request.Description, request.IsSafetyResponse, request.SafetyAckAction, request.MessageType, request.Status, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
            return response;
        }
    }
}
