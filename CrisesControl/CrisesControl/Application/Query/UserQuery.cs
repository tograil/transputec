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

namespace CrisesControl.Api.Application.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMapper _mapper;
        //private readonly GetAllUserValidator _getUsersValidator;
        //private readonly GetUserValidator _getUserValidator;
        //private readonly LoginValidator _loginValidator;
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
           
            var mappedRequest = _mapper.Map<Core.Users.GetAllUserRequestList>(request);
            var users = await _UserRepository.GetAllUsers(mappedRequest);
            List<GetUserResponse> response = _mapper.Map<List<User>, List<GetUserResponse>>(users.ToList());
            var result = new GetAllUserResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request, CancellationToken cancellationToken)
        {
            var User = await _UserRepository.GetUser(request.CompanyId, request.UserId);
            GetUserResponse response = _mapper.Map<User, GetUserResponse>(User);

            return response;
        }

        public async Task<LoginResponse> GetLoggedInUserInfo(LoginRequest request, CancellationToken cancellationToken)
        {
           
            var loginRequest = _mapper.Map<LoginInfo>(request);
            var LoginInfo = await _UserRepository.GetLoggedInUserInfo(loginRequest, cancellationToken);
            var result = _mapper.Map<LoginResponse>(LoginInfo);
            return result;
        }

        public async Task<ActivateUserResponse> ReactivateUser(int queriedUserId, CancellationToken cancellationToken)
        {
            var reactivate = await _UserRepository.ReactivateUser(queriedUserId, cancellationToken);
            var result = _mapper.Map<ActivateUserResponse>(reactivate);
            return result;
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
            var validateEmail = _UserRepository.ValidateLoginEmail(request.UserEmail);
            var result = _mapper.Map<ValidateEmailResponse>(validateEmail.Result);
            return result;
        }

        public async Task<GetAllUserDevicesResponse> GetAllUserDeviceList(GetAllUserDevicesRequest request, CancellationToken cancellationToken)
        {
            GetAllUserDeviceRequest requestMapped = _mapper.Map<GetAllUserDeviceRequest>(request);
            var getAllUserDevices = await _UserRepository.GetAllUserDeviceList(requestMapped, cancellationToken);
            var result = _mapper.Map<List<GetAllUserDevices>>(getAllUserDevices);
            var response = new GetAllUserDevicesResponse();
            response.Data = result;
            return response;
        }

        public async Task<IEnumerable<GetUserCommsResponse>> GetUserComms(GetUserCommsRequest request, CancellationToken cancellationToken)
        {
            var response = await _UserRepository.GetUserComms(request.CommsUserId, cancellationToken);
            var result = _mapper.Map<List<UserComm>, List<GetUserCommsResponse>>(response);
            return result;
        }
        public async Task<IEnumerable<GetAllOneUserDeviceListResponse>> GetAllOneUserDeviceList(GetAllOneUserDeviceListRequest request, CancellationToken cancellationToken)
        {
            var response = await _UserRepository.GetAllOneUserDeviceList(request.QueriedUserId, cancellationToken);
            var result = _mapper.Map<List<GetAllOneUserDeviceListResponse>>(response);
            return result;
        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var resetPassword = await _UserRepository.ResetPassword(_currentUser.CompanyId,_currentUser.UserId,request.OldPassword,request.NewPassword);
                var result = _mapper.Map<string>(resetPassword);
                var response = new ResetPasswordResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.ResetPassword = result;
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
                var result = _mapper.Map<string>(forgotPassword);
                var response = new ForgotPasswordResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Message = result;
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
                var result = _mapper.Map<string>(forgotPassword);
                var response = new LinkResetPasswordResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Message = result;
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
