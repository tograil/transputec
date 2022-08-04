using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPingInfo
{
    public class GetPingInfoHandler:IRequestHandler<GetPingInfoRequest, GetPingInfoResponse>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public GetPingInfoHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<GetPingInfoResponse> Handle(GetPingInfoRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPingInfoRequest));
            var result = _messageRepository.GetPingInfo(request.MessageId, request.UserId, request.CompanyId);
            var response = _mapper.Map<GetPingInfoResponse>(result);
            return response;
        }
    }
}
