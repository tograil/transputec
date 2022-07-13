using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ReactivateUser
{
    public class ReactivateUserHandler : IRequestHandler<ReactivateUserRequest, ReactivateUserResponse>
    {
        private readonly ReactivateUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ReactivateUserHandler(ReactivateUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<ReactivateUserResponse> Handle(ReactivateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ReactivateUserRequest));

            var userId = await _userRepository.ReactivateUser(request.QueriedUserId, cancellationToken);
            return new ReactivateUserResponse();
        }
    }
}
