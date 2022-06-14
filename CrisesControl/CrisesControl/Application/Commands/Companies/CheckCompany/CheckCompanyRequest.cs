using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.CheckCompany
{
    public class CheckCompanyRequest:IRequest<CheckCompanyResponse>
    {
       
        public string CompanyName { get; set; }
      
        public string CountryCode { get; set; }
    }
}
