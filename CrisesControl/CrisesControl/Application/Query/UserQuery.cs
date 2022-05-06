using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMapper _mapper;
        public UserQuery(IUserRepository UserRepository, IMapper mapper)
        {
            _UserRepository = UserRepository;
            _mapper =  mapper;
        }

        public async Task<GetUsersResponse> GetUsers(GetUsersRequest request)
        {
            var Users = await _UserRepository.GetAllUsers(request.CompanyId);
            List<GetUserResponse> response = _mapper.Map<List<User>, List<GetUserResponse>>(Users.ToList());
            var result = new GetUsersResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
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
    }
}
