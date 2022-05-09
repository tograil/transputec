using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompany
{
    public class UpdateCompanyRequest :  IRequest<UpdateCompanyResponse>
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Website { get; set; }
        public string PhoneISDCode { get; set; }
        public string SwitchBoardPhone { get; set; }
        public int TimeZone { get; set; }
        public string Fax { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        
        public string City { get; set; }
        public string State { get; set; }
       
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string CompanyProfile { get; set; }
    }
}
