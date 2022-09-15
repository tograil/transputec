using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserRequest, GetUserResponse>
    {

        private readonly GetUserValidator _userValidator;
        //private readonly IUserQuery _userQuery;
        private readonly IUserRepository _userQuery;
        private readonly ILogger<GetUserHandler> _logger;

        public GetUserHandler(GetUserValidator userValidator, IUserRepository userService, ILogger<GetUserHandler> logger)
        {
            _userValidator = userValidator;
            _userQuery = userService;
            _logger = logger;
        }
        public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserRequest));
            await _userValidator.ValidateAndThrowAsync(request, cancellationToken);
            var User = await _userQuery.GetUser(request.CompanyId, request.UserId);
            var response = new GetUserResponse();
            response.ActiveOffDuty = User.ActiveOffDuty;
            response.CompanyId = User.CompanyId;
            response.CreatedBy = User.CreatedBy;
            response.CreatedOn = User.CreatedOn;
            response.DepartmentId = User.DepartmentId;
            response.ExpirePassword = User.ExpirePassword ?? false;
            response.FirstLogin = User.FirstLogin;
            response.FirstName = User.FirstName;
            response.ISDCode = User.Isdcode;
            response.Landline = User.Landline;
            response.LastLocationUpdate = User.LastLocationUpdate;
            response.LastName = User.LastName;
            response.Lat = User.Lat;
            response.LLISDCode = User.Llisdcode;
            response.Lng = User.Lng;
            response.MobileNo = User.MobileNo;
            response.OTPCode = User.Otpcode;
            response.OTPExpiry = User.Otpexpiry;
            response.Password = User.Password;
            response.PasswordChangeDate = User.PasswordChangeDate;
            response.PrimaryEmail = User.PrimaryEmail;
            response.RegisteredUser = User.RegisteredUser;
            response.SecondaryEmail = User.SecondaryEmail;
            response.SMSTrigger = User.Smstrigger;
            response.Status = User.Status;
            response.TimezoneId = User.TimezoneId;
            response.TrackingEndTime = User.TrackingEndTime;
            response.TrackingStartTime = User.TrackingStartTime;
            response.UniqueGuiID = User.UniqueGuiId;
            response.UpdatedBy = User.UpdatedBy;
            response.UpdatedOn = User.UpdatedOn;
            response.UserHash = User.UserHash;
            response.UserId = User.UserId;
            response.UserLanguage = User.UserLanguage;
            response.UserPhoto = User.UserPhoto;
            response.UserRole = User.UserRole;
            return response;
        }
    }
}
