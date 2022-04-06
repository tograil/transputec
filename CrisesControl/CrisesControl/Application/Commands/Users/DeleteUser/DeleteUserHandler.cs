using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUser
{
    public class DeleteUserHandler: IRequestHandler<DeleteUserRequest, DeleteUserResponse>
    {
        private readonly DeleteUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mappper;

        public DeleteUserHandler(DeleteUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mappper = mapper;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteUserRequest));

            User value = _mappper.Map<DeleteUserRequest,User>(request);
            var user = await _userRepository.DeleteUser(value, cancellationToken);
            var result = new DeleteUserResponse();
            result.UserId = user.UserId;   
            return result;
        }
    }
}
