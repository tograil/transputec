using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateProfile;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using CrisesControl.Api.Application.ViewModels.User;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Maps
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserResponse>();

            CreateMap<CreateUserRequest, User>()
                .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTimeOffset.Now))
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));

            CreateMap<UpdateUserRequest, User>()
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));
            CreateMap<LoginRequest, LoginInfo>();
            CreateMap<LoginInfoReturnModel, LoginResponse>();
            CreateMap<GetUsersRequest, GetAllUserRequest>();

            CreateMap<UpdateProfileRequest, User>();
        }
    }
}
