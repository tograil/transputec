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
        private readonly ILogger<UpdateUserHandler> _logger;
        private readonly ICurrentUser _currentUser;


        public UpdateUserHandler(UpdateUserValidator userValidator, IUserRepository userService, ILogger<UpdateUserHandler> logger, ICurrentUser currentUser)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger=logger;
            _currentUser = currentUser;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateUserRequest));

            User value = new User();  //_mappper.Map<UpdateUserRequest,User>(request);
           value.ActiveOffDuty = request.ActiveOffDuty;
           value.CompanyId = request.CompanyId;
           value.CreatedBy = request.CreatedBy;
           value.CreatedOn = request.CreatedOn;
           value.DepartmentId = request.DepartmentId;
           value.ExpirePassword = request.ExpirePassword ?? true;
           value.FirstLogin = request.FirstLogin;
           value.FirstName = request.FirstName;
           value.Isdcode = request.Isdcode;
           value.Landline = request.Landline;
           value.LastLocationUpdate = request.LastLocationUpdate;
           value.LastName = request.LastName;
           value.Lat = request.Lat;
           value.Llisdcode = request.Llisdcode;
           value.Lng = request.Lng;
           value.MobileNo = request.MobileNo;
           value.Otpcode = request.Otpcode;
           value.Otpexpiry = request.Otpexpiry;
           value.Password = request.Password;
           value.PasswordChangeDate = request.PasswordChangeDate;
           value.PrimaryEmail = request.PrimaryEmail;
           value.RegisteredUser = request.RegisteredUser;
           value.SecondaryEmail = request.SecondaryEmail;
           value.Smstrigger = request.Smstrigger;
           value.Status = request.Status;
           value.TimezoneId = request.TimezoneId;
           value.TrackingEndTime = request.TrackingEndTime;
           value.TrackingStartTime = request.TrackingStartTime;
           value.UniqueGuiId = request.UniqueGuiId;
           value.UpdatedBy = request.UpdatedBy;
           value.UpdatedOn = request.UpdatedOn;
           value.UserHash = request.UserHash;
           value.UserId = request.UserId;
           value.UserLanguage = request.UserLanguage;
           value.UserPhoto = request.UserPhoto;
           value.UserRole = request.UserRole;
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
