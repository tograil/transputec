namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserResponse
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
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public string UserPhoto { get; set; }
        public string ISDCode { get; set; }
        public string UserRole { get; set; }
        public string LLISDCode { get; set; }
        public string Landline { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public DateTimeOffset PasswordChangeDate { get; set; }
        public bool ExpirePassword { get; set; }
        public string UserLanguage { get; set; }
        public bool FirstLogin { get; set; }
        public string OTPCode { get; set; }
        public DateTimeOffset OTPExpiry { get; set; }
        public DateTimeOffset LastLocationUpdate { get; set; }
        public DateTimeOffset TrackingStartTime { get; set; }
        public DateTimeOffset TrackingEndTime { get; set; }
        public int ActiveOffDuty { get; set; }
        public bool SMSTrigger { get; set; }
        public int? DepartmentId { get; set; }
        public string UserHash { get; set; }
        public int? TimezoneId { get; set; }
    }
}