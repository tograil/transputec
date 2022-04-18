using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Commands.Companies.TempRegister;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Maps;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<TempRegisterRequest, Registration>()
            .ForMember(x => x.UniqueReference, m => m.MapFrom(x => Guid.NewGuid().ToString("D")))
            .ForMember(x => x.Status, m => m.MapFrom(x => 1))
            .ForMember(x => x.CountryCode, m => m.MapFrom(x => "GBR"))
            .ForMember(x => x.MobileIsd, m => m.MapFrom(x => "+44"))
            .ForMember(x => x.CreatedOn, m => m.MapFrom(x => DateTime.Now.GetDateTimeOffset("GMT Standard Time")));

        CreateMap<GetCompanyRequest, CompanyRequestInfo>();
        CreateMap<CompanyInfoReturn, GetCompanyResponse>();
    }
}