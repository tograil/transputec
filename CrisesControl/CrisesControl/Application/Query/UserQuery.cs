﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
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
    }
}