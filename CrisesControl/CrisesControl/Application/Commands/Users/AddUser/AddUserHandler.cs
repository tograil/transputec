using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.SharedKernel.Utils;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.AddUser
{
    public class AddUserHandler: IRequestHandler<AddUserRequest, AddUserResponse>
    {
        private readonly AddUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddUserHandler> _logger;
        private readonly ICurrentUser _currentUser;

        public AddUserHandler(AddUserValidator userValidator, IUserRepository userService, ILogger<AddUserHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<AddUserResponse> Handle(AddUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AddUserRequest));

            User value = new User()
            {
                ActiveOffDuty = request.ActiveOffDuty,
                CompanyId = request.CompanyId,
                CreatedBy = _currentUser.UserId,
                CreatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone),
                DepartmentId = request.DepartmentId,
                ExpirePassword = request.ExpirePassword,
                FirstLogin = true,
                FirstName = request.FirstName,
                Isdcode = request.Isdcode,
                Landline = request.Landline,
                LastLocationUpdate = request.LastLocationUpdate,
                LastName = request.LastName,
                Lat = request.Lat,
                Llisdcode = request.Llisdcode,
                Lng = request.Lng,
                MobileNo = request.MobileNo,
                Otpcode = request.Otpcode,
                Otpexpiry = request.Otpexpiry,
                Password = request.Password,
                PasswordChangeDate = request.PasswordChangeDate,
                PrimaryEmail = request.PrimaryEmail,
                RegisteredUser = request.RegisteredUser,
                SecondaryEmail = request.SecondaryEmail,
                Smstrigger = false,
                Status = 2,
                TimezoneId = request.TimezoneId,
                TrackingEndTime = request.TrackingEndTime,
                TrackingStartTime = request.TrackingStartTime,
                UniqueGuiId = request.UniqueGuiId,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(_currentUser.TimeZone),
                UserHash = request.UserHash,
                UserLanguage = request.UserLanguage,
                UserPhoto = request.UserPhoto,
                UserRole = request.UserRole

        };   // _mapper.Map<AddUserRequest, User>(request);
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
