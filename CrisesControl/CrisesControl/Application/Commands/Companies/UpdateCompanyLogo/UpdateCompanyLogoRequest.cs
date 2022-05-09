using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyLogo
{
    public class UpdateCompanyLogoRequest : IRequest<UpdateCompanyLogoResponse>
    {
        public int CompanyId { get; set; }
        public string CompanyLogo { get; set; }
        public string iOSLogo { get; set; }
        public string AndroidLogo { get; set; }
        public string WindowsLogo { get; set; }
        public string LogoType { get; set; }
    }
}
