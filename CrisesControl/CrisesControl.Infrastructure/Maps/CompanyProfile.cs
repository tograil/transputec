using System;
using System.Globalization;
using AutoMapper;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Extensions.Configuration;

namespace CrisesControl.Infrastructure.Maps;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<TempCompanyRegister, Registration>()
            .ForMember(x => x.UniqueReference, m => m.MapFrom(x => Guid.NewGuid().ToString("D")))
            .ForMember(x => x.Status, m => m.MapFrom(x => 1))
            .ForMember(x => x.CountryCode, m => m.MapFrom(x => "GBR"))
            .ForMember(x => x.MobileIsd, m => m.MapFrom(x => "+44"))
            .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTime.Now.GetDateTimeOffset("GMT Standard Time")));
    }
}