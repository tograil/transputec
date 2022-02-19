using AutoMapper;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.Models;
using Microsoft.Extensions.Configuration;

namespace CrisesControl.Infrastructure.Maps;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<TempCompanyRegisterRoot, Registration>()
            .ForMember(x => x.UniqueReference, m => m.MapFrom(x => x.UniqueRef));
    }
}