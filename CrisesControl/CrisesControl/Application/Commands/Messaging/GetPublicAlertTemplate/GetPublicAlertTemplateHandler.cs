using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPublicAlertTemplate
{
    public class GetPublicAlertTemplateHandler : IRequestHandler<GetPublicAlertTemplateRequest, GetPublicAlertTemplateResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public GetPublicAlertTemplateHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<GetPublicAlertTemplateResponse> Handle(GetPublicAlertTemplateRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPublicAlertTemplateRequest));
            var result = _messageRepository.GetPublicAlertTemplate(request.MessageId, request.UserId, request.CompanyId);
            var response = _mapper.Map<GetPublicAlertTemplateResponse>(result);
            return response;
        }
    }
}
