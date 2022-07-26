using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.DownloadDeliveryReport
{
    public class DownloadDeliveryReportRequest:IRequest<DownloadDeliveryReportResponse>
    {
        public int MessageID { get; set; }
    }
}
