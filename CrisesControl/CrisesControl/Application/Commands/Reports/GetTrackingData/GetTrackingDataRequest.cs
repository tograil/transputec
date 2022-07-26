using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingData
{
    public class GetTrackingDataRequest:IRequest<GetTrackingDataResponse>
    {
        public int TrackMeID { get; set; }
        public int UserDeviceID { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        
    }
}
