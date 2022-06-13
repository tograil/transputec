using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUser
{
    public class UpdateUserHandler: IRequestHandler<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly UpdateUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mappper;
        private readonly ICurrentUser _currentUser;


        public UpdateUserHandler(UpdateUserValidator userValidator, IUserRepository userService, IMapper mapper, ICurrentUser currentUser)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mappper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateUserRequest));

            User value = _mappper.Map<UpdateUserRequest,User>(request);
            if (CheckDuplicate(value))
            {
                var userId = await _userRepository.UpdateUser(value, cancellationToken);
                var result = new UpdateUserResponse();
                result.UserId = userId;   
                return result;
            } 
            throw new UserNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }

        private bool CheckDuplicate(User user)
        {
            return _userRepository.CheckDuplicate(user);
        }
    }
}
