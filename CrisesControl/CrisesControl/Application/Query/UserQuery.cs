using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;

namespace CrisesControl.Api.Application.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMapper _mapper;
        private readonly GetUsersValidator _getUsersValidator;
        private readonly GetUserValidator _getUserValidator;
        private readonly LoginValidator _loginValidator;
        private readonly ILogger<UserQuery> _logger;
        private readonly IPaging _paging;
        public UserQuery(IUserRepository UserRepository, IMapper mapper, ILogger<UserQuery> logger, IPaging paging, GetUsersValidator getUsersValidator, GetUserValidator getUserValidator, LoginValidator loginValidator)
        {
            _UserRepository = UserRepository;
            _mapper =  mapper;
            _getUsersValidator = getUsersValidator;
            _getUserValidator = getUserValidator;
            _loginValidator = loginValidator;
            _logger = logger;
            _paging = paging;
        }

        public async Task<GetUsersResponse> GetUsers(GetUsersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserRequest));

            await _getUsersValidator.ValidateAndThrowAsync(request, cancellationToken);

            var mappedRequest = _mapper.Map<GetAllUserRequest>(request);
            var users = await _UserRepository.GetAllUsers(mappedRequest);
            List<GetUserResponse> response = _mapper.Map<List<User>, List<GetUserResponse>>(users.ToList());
            var result = new GetUsersResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserRequest));

            await _getUserValidator.ValidateAndThrowAsync(request, cancellationToken);

            var User = await _UserRepository.GetUser(request.CompanyId, request.UserId);
            GetUserResponse response = _mapper.Map<User, GetUserResponse>(User);

            return response;
        }

        public async Task<LoginResponse> GetLoggedInUserInfo(LoginRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(LoginRequest));
            await _loginValidator.ValidateAndThrowAsync(request, cancellationToken);

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
                rtn.recordsFiltered = membership.Count();
                rtn.data = membership;
                int totalRecord = membership.Count();
                rtn.draw = request.Draw;
                rtn.recordsTotal = totalRecord;


                return new MembershipResponse
                {
                    recordsFiltered = rtn.recordsFiltered,
                    data = rtn.data,
                    draw = request.Draw,
                    recordsTotal = rtn.recordsTotal,

                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured whikle trying to seed the database {0},{1},{2},{3}", ex.Message, ex.InnerException, ex.StackTrace, ex.Source);
            }
            return new MembershipResponse { };
        }
    }
}
