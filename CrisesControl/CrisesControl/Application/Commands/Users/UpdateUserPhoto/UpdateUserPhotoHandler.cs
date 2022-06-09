using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhoto
{
    public class UpdateUserPhotoHandler: IRequestHandler<UpdateUserPhotoRequest, UpdateUserPhotoResponse>
    {
        private readonly UpdateUserPhotoValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mappper;
        private readonly ICurrentUser _currentUser;


        public UpdateUserPhotoHandler(UpdateUserPhotoValidator userValidator, IUserRepository userService, IMapper mapper, ICurrentUser currentUser)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mappper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UpdateUserPhotoResponse> Handle(UpdateUserPhotoRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateUserPhotoRequest));

            User value = _mappper.Map<UpdateUserPhotoRequest,User>(request);
            if (CheckDuplicate(value))
            {
                var user = await _userRepository.UpdateUserPhoto(value, request.UserPhoto, cancellationToken);
                var result = new UpdateUserPhotoResponse();
                result.UserId = user.UserId;   
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
