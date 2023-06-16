using AutoMapper;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.DeleteUser;
using CrisesControl.Api.Application.ViewModels.User;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Maps
{
    public class DeleteUserProfile : Profile
    {
        public DeleteUserProfile()
        {
            CreateMap<DeleteUserRequest, User>();
        }
    }
}
