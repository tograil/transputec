namespace CC.Authority.Core.UserManagement.Models
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public bool RegisteredUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string UniqueGuiID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int Status { get; set; }
        public string UserPhoto { get; set; }
        public string ISDCode { get; set; }
        public string UserRole { get; set; }
        public string LLISDCode { get; set; }
        public string? Landline { get; set; }
        public string? Lat { get; set; }
        public string Lng { get; set; }
        public string UserLanguage { get; set; }
        public bool FirstLogin { get; set; }
        public string OtpCode { get; set; }
        public string ExternalScimId { get; set; }
    }
}