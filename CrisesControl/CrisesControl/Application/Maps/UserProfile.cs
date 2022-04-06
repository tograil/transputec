using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.ViewModels.User;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Maps
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserResponse>();
        }
    }
}
