using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUserComms;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList;
using CrisesControl.Api.Application.Commands.Users.GetAllUser;
using CrisesControl.Api.Application.Commands.Users.ResetPassword;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Commands.Users.ForgotPassword;
using CrisesControl.Api.Application.Commands.Users.LinkResetPassword;
using CrisesControl.SharedKernel.Utils;
using CrisesControl.Api.Application.Commands.Users.GetUserId;
using CrisesControl.Api.Application.Commands.Users.GetUserGroups;

namespace CrisesControl.Api.Application.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserQuery> _logger;
        private readonly IPaging _paging;
        private readonly ICurrentUser _currentUser;
        public UserQuery(IUserRepository UserRepository, IMapper mapper, ILogger<UserQuery> logger, IPaging paging, ICurrentUser currentUser)
        {
            _UserRepository = UserRepository;
            _mapper =  mapper;
            _logger = logger;
            _paging = paging;
            _currentUser = currentUser;
        }

        public async Task<GetAllUserResponse> GetUsers(Commands.Users.GetAllUser.GetAllUserRequest request, CancellationToken cancellationToken)
        {

           
            GetAllUserRequestList getAllUser = new GetAllUserRequestList();
            getAllUser.ActiveOnly = request.ActiveOnly;
            getAllUser.CompanyKey = request.CompanyKey;
            getAllUser.Filters = request.Filters;
            getAllUser.KeyHolderOnly = request.KeyHolderOnly;
            getAllUser.OrderBy = _paging.OrderBy;
            getAllUser.OrderDir = request.OrderDir;
            getAllUser.RecordLength = _paging.PageSize;
            getAllUser.RecordStart = _paging.PageNumber;
            getAllUser.SearchString = request.SearchString;
            getAllUser.SkipDeleted = request.SkipDeleted;
            getAllUser.SkipInActive = request.SkipInActive;
            var users = await _UserRepository.GetAllUsers(getAllUser);
            var result = _mapper.Map<List<User>>(users);
            var response = new GetAllUserResponse();
            response.Data = result;
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request, CancellationToken cancellationToken)
        {
            var User = await _UserRepository.GetUser(request.CompanyId, request.UserId);
            var response = new GetUserResponse();    //_mapper.Map<User, GetUserResponse>(User);
            response.ActiveOffDuty = User.ActiveOffDuty;
            response.CompanyId = User.CompanyId;
            response.CreatedBy = User.CreatedBy;
            response.CreatedOn = User.CreatedOn;
            response.DepartmentId = User.DepartmentId;
            response.ExpirePassword = User.ExpirePassword ?? true;
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

        public async Task<LoginResponse> GetLoggedInUserInfo(LoginRequest request, CancellationToken cancellationToken)
        {

            LoginInfo loginInfo = new LoginInfo();
            loginInfo.DeviceType = request.DeviceType;
            loginInfo.IPAddress = request.IPAddress;
            loginInfo.Language = request.Language;
            var User = await _UserRepository.GetLoggedInUserInfo(loginInfo, cancellationToken);
            // var result = _mapper.Map<LoginResponse>(LoginInfo);
            var response = new LoginResponse();
            response.ActiveOffDuty = User.ActiveOffDuty;
            response.CompanyId = User.CompanyId;
            response.CustomerId = User.CustomerId;
            response.FirstLogin = User.FirstLogin;
            response.First_Name = User.First_Name;
            response.UserMobileISD = User.UserMobileISD;
            response.Message = User.Message;
            response.PortalTimeZone = User.PortalTimeZone;
            response.Last_Name = User.Last_Name;
            response.SecItems = User.SecItems;
            response.MobileNo = User.MobileNo;
            response.UserPassword = User.UserPassword;
            response.Primary_Email = User.Primary_Email;
            response.UniqueGuiId = User.UniqueGuiId;
            response.UniqueKey = User.UniqueKey;
            response.Status = User.Status;
            response.TimeZoneId = User.TimeZoneId;
            response.UserMobileISD = User.UserMobileISD;
            response.UserId = User.UserId;
            response.UserLanguage = User.UserLanguage;
            response.UserPhoto = User.UserPhoto;
            response.UserRole = User.UserRole;
            response.CompanyName = User.CompanyName;
            response.CompanyLogo = User.CompanyLogo;
            response.AnniversaryDate = User.AnniversaryDate;
            response.Activation = User.Activation;
            response.CompanyPlanId = User.CompanyPlanId;
            response.CompanyProfile = User.CompanyProfile;
            response.CompanyStatus = User.CompanyStatus;
            response.ErrorId = User.ErrorId;
            return response;
        }

        public async Task<ActivateUserResponse> ReactivateUser(int queriedUserId, CancellationToken cancellationToken)
        {
            var User = await _UserRepository.ReactivateUser(queriedUserId, cancellationToken);
            // var result = _mapper.Map<ActivateUserResponse>(reactivate);
            var response = new ActivateUserResponse();
            response.ActiveOffDuty = User.ActiveOffDuty;
            response.CompanyId = User.CompanyId;
            response.CreatedBy = User.CreatedBy;
            response.CreatedOn = User.CreatedOn;
            response.DepartmentId = User.DepartmentId;
            response.ExpirePassword = User.ExpirePassword ?? true;
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

        public async Task<MembershipResponse> MembershipList(MembershipRequest request)
        {
            try
            {

                var membership = await _UserRepository.MembershipList(request.ObjMapID, request.MemberShipType, request.TargetID, _paging.PageNumber, _paging.PageSize, request.search, _paging.OrderBy, request.OrderDir, _paging.Apply, request.CompanyKey);
                DataTablePaging rtn = new DataTablePaging();
                rtn.RecordsFiltered = membership.Count();
                rtn.Data = membership;
                int totalRecord = membership.Count();
                rtn.Draw = request.Draw;
                rtn.RecordsTotal = totalRecord;


                return new MembershipResponse
                {
                    recordsFiltered = rtn.RecordsFiltered,
                    data = rtn.Data,
                    draw = request.Draw,
                    recordsTotal = rtn.RecordsTotal,

                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured whikle trying to seed the database {0},{1},{2},{3}", ex.Message, ex.InnerException, ex.StackTrace, ex.Source);
            }
            return new MembershipResponse { };
        }

        public async Task<ValidateEmailResponse> ValidateLoginEmail(ValidateEmailRequest request)
        {
            var validateEmail =await _UserRepository.ValidateLoginEmail(request.UserEmail);
            //var result = _mapper.Map<ValidateEmailResponse>(validateEmail.Result);
            var response = new ValidateEmailResponse();
            response.SSOEnabled = validateEmail.SSOEnabled;
            response.SSOIssuer = validateEmail.SSOIssuer;
            response.SSOSecret = validateEmail.SSOSecret;
            response.SSOType = validateEmail.SSOType;
            
            return response;
        }

        public async Task<GetAllUserDevicesResponse> GetAllUserDeviceList(GetAllUserDevicesRequest request, CancellationToken cancellationToken)
        {
            //GetAllUserDeviceRequest requestMapped = _mapper.Map<GetAllUserDeviceRequest>(request);
            var getAllUserDevices = await _UserRepository.GetAllUserDeviceList(request.DeviceRequest, cancellationToken);
           // var result = _mapper.Map<List<GetAllUserDevices>>(getAllUserDevices);
            var response = new GetAllUserDevicesResponse();
            response.Data = getAllUserDevices;
            return response;
        }

        public async Task<GetUserCommsResponse> GetUserComms(GetUserCommsRequest request, CancellationToken cancellationToken)
        {
            var result = await _UserRepository.GetUserComms(request.CommsUserId, cancellationToken);
            //var result = _mapper.Map<List<UserComm>, List<GetUserCommsResponse>>(response);
            var response = new GetUserCommsResponse();
            response.MessageType = result.FirstOrDefault().MessageType;
            response.MethodId = result.FirstOrDefault().MethodId;
            return response;
        }
        public async Task<GetAllOneUserDeviceListResponse> GetAllOneUserDeviceList(GetAllOneUserDeviceListRequest request, CancellationToken cancellationToken)
        {
            var userDeviceLists = await _UserRepository.GetAllOneUserDeviceList(request.QueriedUserId, cancellationToken);
            //var result = _mapper.Map<List<GetAllOneUserDeviceListResponse>>(response);
            var response = new GetAllOneUserDeviceListResponse();
            response.CompanyId = userDeviceLists.FirstOrDefault().CompanyID;
            response.DeviceID = userDeviceLists.FirstOrDefault().DeviceId;
            response.DeviceModel = userDeviceLists.FirstOrDefault().DeviceModel;
            response.DeviceOS = userDeviceLists.FirstOrDefault().DeviceOs;
            response.DeviceSerial = userDeviceLists.FirstOrDefault().DeviceSerial;
            response.DeviceType = userDeviceLists.FirstOrDefault().DeviceType;
            response.ExtraInfo = userDeviceLists.FirstOrDefault().ExtraInfo;
            response.LastLoginFrom = userDeviceLists.FirstOrDefault().LastLoginFrom;
            response.OverrideSilent = userDeviceLists.FirstOrDefault().OverrideSilent;
            response.SirenON = userDeviceLists.FirstOrDefault().SirenOn;
            response.Status = userDeviceLists.FirstOrDefault().Status;
            response.UserDeviceID = userDeviceLists.FirstOrDefault().UserDeviceID;
            response.UserID = userDeviceLists.FirstOrDefault().UserId;
            response.UserName = userDeviceLists.FirstOrDefault().UserName;
            return response;
        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var resetPassword = await _UserRepository.ResetPassword(_currentUser.CompanyId,_currentUser.UserId,request.OldPassword,request.NewPassword);
                //var result = _mapper.Map<string>(resetPassword);
                var response = new ResetPasswordResponse();
                if (!string.IsNullOrEmpty(resetPassword))
                {
                    response.ResetPassword = resetPassword;
                }
                else
                {
                    response.ResetPassword = "No data found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }


        public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var forgotPassword = await _UserRepository.ForgotPassword(request.EmailId,request.Method.ToDbString(),request.CustomerId,request.OTPMessage,request.Return,_currentUser.CompanyId,_currentUser.TimeZone,request.Source);
               // var result = _mapper.Map<string>(forgotPassword);
                var response = new ForgotPasswordResponse();
                if (!string.IsNullOrEmpty(forgotPassword))
                {
                    response.Message = forgotPassword;
                }
                else
                {
                    response.Message = "No data found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LinkResetPasswordResponse> LinkResetPassword(LinkResetPasswordRequest request)
        {
            try
            {
                var forgotPassword = await _UserRepository.LinkResetPassword(_currentUser.CompanyId, request.QueriedGuid,request.NewPassword,_currentUser.TimeZone);
                //var result = _mapper.Map<string>(forgotPassword);
                var response = new LinkResetPasswordResponse();
                if (!string.IsNullOrEmpty(forgotPassword))
                {
                    response.Message = forgotPassword;
                }
                else
                {
                    response.Message = "No data found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GetUserIdResponse> GetUserId(GetUserIdRequest request)
        {
            try
            {
                var userId = await _UserRepository.GetUserId(_currentUser.CompanyId,request.EmailAddress);
                // var result = _mapper.Map<string>(forgotPassword);
                var response = new GetUserIdResponse();
                if (userId != null)
                {
                    response.User = userId;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Message = "No data found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUserGroupsResponse> GetUserGroups(GetUserGroupsRequest request)
        {
            try
            {
                var userId = await _UserRepository.GetUserGroups(request.UserId);
                // var result = _mapper.Map<string>(forgotPassword);
                var response = new GetUserGroupsResponse();
                if (userId != null)
                {
                    response.UserGroups = userId;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Message = "No data found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
