using AutoMapper;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog;
using CrisesControl.Core.CustomEventLog;

namespace CrisesControl.Api.Application.Maps
{
    public class CustomEventLogProfile : Profile
    {
        public CustomEventLogProfile()
        {
            CreateMap<EventLogListing, GetEventLogResponse>();
            CreateMap<GetEventLogResponse, EventLogListing>();
        }
    }
}
