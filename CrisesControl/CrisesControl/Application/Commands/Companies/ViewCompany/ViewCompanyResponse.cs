using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.ViewCompany;

public class ViewCompanyResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CompanyName { get; set; }
    public string CompanyLogo { get; set; }
    public string ContactLogo { get; set; }
    public string Website { get; set; }
    public string MasterActionPlan { get; set; }
    public string PrimaryEmail { get; set; }
    public string PhoneISDCode { get; set; }
    public string SwitchBoardPhone { get; set; }
    public string Fax { get; set; }
    public string TimeZone { get; set; }
    public HttpStatusCode ErrorId { get; set; }
    public string Message { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Postcode { get; set; }
    public string CountryCode { get; set; }
    public string CompanyProfile { get; set; }
    public DateTimeOffset AnniversaryDate { get; set; }
    public bool OnTrial { get; set; }
    public string CustomerId { get; set; }
    public string InvitationCode { get; set; }
}
