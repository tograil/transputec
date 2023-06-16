using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPublicAlertTemplate
{
    public class GetPublicAlertTemplateHandler : IRequestHandler<GetPublicAlertTemplateRequest, GetPublicAlertTemplateResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public GetPublicAlertTemplateHandler(IMessageRepository messageRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<GetPublicAlertTemplateResponse> Handle(GetPublicAlertTemplateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPublicAlertTemplateRequest));
            var result = _messageRepository.GetPublicAlertTemplate(request.MessageId, _currentUser.UserId, _currentUser.CompanyId);
            var response = _mapper.Map<GetPublicAlertTemplateResponse>(result);
            return response;
        }
    }
}
