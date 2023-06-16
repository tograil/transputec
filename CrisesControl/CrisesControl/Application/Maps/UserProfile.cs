using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.AddUser;
using CrisesControl.Api.Application.Commands.Users.GetAllUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUserComms;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateProfile;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
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

            CreateMap<AddUserRequest, User>()
                .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTimeOffset.Now))
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));

            CreateMap<UpdateUserRequest, User>()
                .ForMember(x => x.UpdatedOn, m => m.MapFrom(x => DateTimeOffset.Now));
            CreateMap<LoginRequest, LoginInfo>();
            CreateMap<LoginInfoReturnModel, LoginResponse>();
            CreateMap<GetAllUserRequest, GetAllUserRequestList>();

            CreateMap<UpdateProfileRequest, User>();
            CreateMap<ValidateEmailReponseModel, ValidateEmailResponse>();

            CreateMap<GetUserCommsResponse, UserComm>()
                .ForMember(x=>x.MethodId, m=>m.MapFrom(x=> x.MethodId))
                .ForMember(x=>x.MessageType, m=>m.MapFrom(x=>x.MessageType));
        }
    }
}
