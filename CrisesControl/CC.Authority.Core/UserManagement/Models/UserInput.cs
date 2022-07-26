namespace CC.Authority.Core.UserManagement.Models
{
    public class UserInput
    {
        public string ExternalScimId { get; set; }
        public int CompanyId { get; set; }
        public int CurrentUserId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileISDCode { get; set; }
        public string MobileNo { get; set; }
        public string LLISDCode { get; set; }
        public string Landline { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public int Status { get; set; }
        public int SendInvite { get; set; }
        public bool ExpirePassword { get; set; }
        public string UserLanguage { get; set; }
        public int TimezoneId { get; set; }
    }
}