using AutoMapper;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports.SP_Response;

namespace CrisesControl.Core.Maps;
public class KeyContactProfile : Profile
{
    public KeyContactProfile()
    {
        CreateMap<IncidentDataByActivationRefKeyContactsResponse, KeyContact>()
            .ForMember(x => x.KeyContactName.Firstname, m => m.MapFrom(x => x.KeyContactFirstName))
            .ForMember(x => x.KeyContactName.Lastname, m => m.MapFrom(x => x.KeyContactLastName))
            .ForMember(x => x.KeyContactMob, m => m.MapFrom(x => x.KeyContactMobileNo))
            .ForMember(x => x.KeyLat, m => m.MapFrom(x => x.Lat))
            .ForMember(x => x.KeyLng, m => m.MapFrom(x => x.Lng));
    }
}
