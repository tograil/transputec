using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserTracking
{
    public class GetUserTrackingResponse
    {
        public long UserLocationID { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string UserPhoto { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserFullName UserName { get; set; }
        public string LocationAddress { get; set; }
        public string PrimaryEmail { get; set; }
        public string MobileNo { get; set; }
        public string ISDCode { get; set; }
        public DateTimeOffset UserDeviceTime { get; set; }
    }
}
