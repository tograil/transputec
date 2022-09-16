using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPingInfo
{
    public class GetPingInfoHandler:IRequestHandler<GetPingInfoRequest, GetPingInfoResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public GetPingInfoHandler(IMessageRepository messageRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<GetPingInfoResponse> Handle(GetPingInfoRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPingInfoRequest));
            var result = await _messageRepository.GetPingInfo(request.MessageId, _currentUser.UserId, _currentUser.CompanyId);
            var response = _mapper.Map<GetPingInfoResponse>(result);
            return response;
        }
    }
}
