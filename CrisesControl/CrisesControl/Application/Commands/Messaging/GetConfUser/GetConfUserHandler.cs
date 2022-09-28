using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Messages.Repositories;
using CrisesControl.SharedKernel.Utils;
using CrisesControl.Core.Messages;
using CrisesControl.Api.Application.Helpers;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetConfUser
{
    public class GetConfUserHandler: IRequestHandler<GetConfUserRequest, GetConfUserResponse>
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public GetConfUserHandler(IMessageRepository messageRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }
        public async Task<GetConfUserResponse> Handle(GetConfUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetConfUserRequest));
            var response = new GetConfUserResponse();
            var conference = await _messageRepository.GetConfUser(request.ObjectId, request.ObjectType.ToDbMethodString());
            var result =  _mapper.Map<ConferenceUser>(conference);
            response.ConferenceUser = result;
            return response;
        }
    }
}
