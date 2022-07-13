using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.AddUser
{
    public class AddUserHandler: IRequestHandler<AddUserRequest, AddUserResponse>
    {
        private readonly AddUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AddUserHandler(AddUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<AddUserResponse> Handle(AddUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AddUserRequest));

            User value = _mapper.Map<AddUserRequest, User>(request);
            if (!CheckDuplicate(value))
            {
                var userId = await _userRepository.CreateUser(value, cancellationToken);
                var result = new AddUserResponse();
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
