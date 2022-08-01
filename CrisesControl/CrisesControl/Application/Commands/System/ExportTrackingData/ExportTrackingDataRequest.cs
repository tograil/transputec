using MediatR;

namespace CrisesControl.Api.Application.Commands.System.ExportTrackingData
{
    public class ExportTrackingDataRequest:IRequest<ExportTrackingDataResponse>
    {
        public int TrackMeID { get; set; }
        public int UserDeviceID { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
