namespace CrisesControl.Api.Application.ViewModels.Company;

public class CompanyInfo
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string CompanyLogo { get; set; }
    public string SwitchBoardPhone { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
    public int Status { get; set; }
    public string PlanName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PrimaryEmail { get; set; }
    public string IsdCode { get; set; }
    public string MobileNo { get; set; }
    public string CompanyProfile { get; set; }
    public string AgreementNo { get; set; }
    public DateTimeOffset ContractAnniversary { get; set; }
    public bool OnTrial { get; set; }
    public string CustomerId { get; set; }
    public string InvitationCode { get; set; }
}