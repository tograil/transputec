using AutoMapper;
using CC.Authority.Core.UserManagement.Models;

namespace CC.Authority.Implementation.Mapping
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserInput, UserResponse>()
                .ReverseMap();
        }
    }
}