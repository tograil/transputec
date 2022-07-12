using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetRegisteredUser
{
    public class GetRegisteredUserHandler : IRequestHandler<GetRegisteredUserRequest, GetRegisteredUserResponse>
    {
        private readonly GetRegisteredUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetRegisteredUserHandler(GetRegisteredUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<GetRegisteredUserResponse> Handle(GetRegisteredUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetRegisteredUserRequest));

            //var userId = await _userRepository.GetRegisteredUser(request.ModuleId, request.UserId, request.XPos, request.YPos);
            return new GetRegisteredUserResponse();
        }
    }
}
