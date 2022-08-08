using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPublicAlert
{
    public class GetPublicAlertHandler : IRequestHandler<GetPublicAlertRequest, GetPublicAlertResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public GetPublicAlertHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<GetPublicAlertResponse> Handle(GetPublicAlertRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPublicAlertRequest));
            var result = _messageRepository.GetPublicAlert(request.CompanyId, request.UserId);
            var response = _mapper.Map<GetPublicAlertResponse>(result);
            return response;
        }
    }
}
