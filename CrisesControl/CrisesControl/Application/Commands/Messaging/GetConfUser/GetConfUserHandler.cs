using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetConfUser
{
    public class GetConfUserHandler: IRequestHandler<GetConfUserRequest, GetConfUserResponse>
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public GetConfUserHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<GetConfUserResponse> Handle(GetConfUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetConfUserRequest));
            var response = new GetConfUserResponse();
            var result = _messageRepository.GetConfUser(request.ObjectId, request.ObjectType);
            return _mapper.Map<GetConfUserResponse>(result);
        }
    }
}
