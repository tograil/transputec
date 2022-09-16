using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;
using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPublicAlert
{
    public class GetPublicAlertHandler : IRequestHandler<GetPublicAlertRequest, GetPublicAlertResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public GetPublicAlertHandler(IMessageRepository messageRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<GetPublicAlertResponse> Handle(GetPublicAlertRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPublicAlertRequest));
            var publicAlerts = _messageRepository.GetPublicAlert(_currentUser.CompanyId, _currentUser.UserId);
            var result = _mapper.Map<List<PublicAlertRtn>>(publicAlerts);
            var response = new GetPublicAlertResponse();
            response.Data = result;
            return response;
        }
    }
}
