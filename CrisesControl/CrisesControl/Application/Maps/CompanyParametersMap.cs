using AutoMapper;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Commands.CompanyParameters.SegregationOtp;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Maps
{
    public class CompanyParametersMap : Profile
    {
        public CompanyParametersMap()
        {
            CreateMap<CompanyFtp, GetCompanyResponse>();
            CreateMap<OTPResponse, SegregationOtpResponse>();
        }

    }
}
