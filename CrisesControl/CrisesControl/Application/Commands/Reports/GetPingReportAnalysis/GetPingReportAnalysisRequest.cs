using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis
{
    public class GetPingReportAnalysisRequest:IRequest<GetPingReportAnalysisResponse>
    {
        public int MessageId { get; set; }
        public MessageCheckType MessageType { get; set; }
    }
}
