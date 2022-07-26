using CrisesControl.Core.Reports;

namespace CrisesControl.Api.Application.Commands.Reports.GetTrackingData
{
    public class GetTrackingDataResponse
    {
        public List<TrackingExport> Data { get; set; }
        public string Message { get; set; }
    }
}
