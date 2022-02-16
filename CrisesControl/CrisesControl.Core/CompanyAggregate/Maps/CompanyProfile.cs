using AutoMapper;
using CrisesControl.Core.CompanyAggregate.Handlers.TempRegister;

namespace CrisesControl.Core.CompanyAggregate.Maps;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<TempRegisterRequest, TempCompanyRegisterRoot>();
    }
}