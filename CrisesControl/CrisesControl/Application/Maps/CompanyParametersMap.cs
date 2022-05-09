using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Maps
{
    public class CompanyParametersMap:Profile
    {
        public CompanyParametersMap()
        {
            CreateMap<CompanyFtp, GetCompanyResponse>();
        }
      
    }
}
