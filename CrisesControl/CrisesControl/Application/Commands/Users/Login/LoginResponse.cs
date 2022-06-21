using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginResponse
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyProfile { get; set; }
        public DateTimeOffset AnniversaryDate { get; set; }
        public int UserId { get; set; }
        public string CustomerId { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string UserMobileISD { get; set; }
        public string MobileNo { get; set; }
        public string Primary_Email { get; set; }
        public string UserPassword { get; set; }
        public string UserPhoto { get; set; }
        public string UniqueGuiId { get; set; }
        public bool RegisterUser { get; set; }
        public string UserRole { get; set; }
        public string UserLanguage { get; set; }
        public int Status { get; set; }
        public bool FirstLogin { get; set; }
        public int CompanyPlanId { get; set; }
        public int CompanyStatus { get; set; }
        public string UniqueKey { get; set; }
        public string PortalTimeZone { get; set; }
        public int ActiveOffDuty { get; set; }
        public int TimeZoneId { get; set; }
        public int Activation { get; set; }
        public List<SecItemModel> SecItems { get; set; }
        public int ErrorId { get; set; }
        public string Message { get; set; }
    }
}
