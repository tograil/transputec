namespace CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice
{
    public class GetAllUserDevicesRequest
    {
        public int OutUserCompanyId { get; set; }
        public int UserID { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string SearchString { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public string CompanyKey { get; set; }
    }
}
