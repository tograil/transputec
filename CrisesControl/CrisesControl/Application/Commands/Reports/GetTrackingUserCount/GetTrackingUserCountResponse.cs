using CrisesControl.Core.Reports;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount
{
    public class GetTrackingUserCountResponse
    {
        public List<TrackUserCount> Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
