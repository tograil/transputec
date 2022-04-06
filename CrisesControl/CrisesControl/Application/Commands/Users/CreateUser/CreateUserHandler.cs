using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CreateUser
{
    public class CreateUserHandler: IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly CreateUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserHandler(CreateUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateUserRequest));

            User value = _mapper.Map<CreateUserRequest, User>(request);
            if (CheckDuplicate(value))
            {
                var userId = await _userRepository.CreateUser(value, cancellationToken);
                var result = new CreateUserResponse();
                result.UserId = userId;
                return result;
            }
            return null;
        }

        private bool CheckDuplicate(User user)
        {
            return _userRepository.CheckDuplicate(user);
        }
    }
}
