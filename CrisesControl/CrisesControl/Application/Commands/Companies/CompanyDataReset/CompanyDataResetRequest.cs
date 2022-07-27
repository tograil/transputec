using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CompanyDataReset
{
    public class CompanyDataResetRequest:IRequest<CompanyDataResetResponse>
    {
        public string[] ResetOptions { get; set; }
    }
}
