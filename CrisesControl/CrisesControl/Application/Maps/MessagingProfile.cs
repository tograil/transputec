using AutoMapper;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Maps {
    public class MessagingProfile : Profile {
        public MessagingProfile() {

            CreateMap<GetNotificationsCountRequest, UserMessageCount>()
            .ForMember(x => x.CompanyId, m => m.MapFrom(x => 0));

            CreateMap<UserMessageCount, GetNotificationsCountResponse>();
        }
    }
}
