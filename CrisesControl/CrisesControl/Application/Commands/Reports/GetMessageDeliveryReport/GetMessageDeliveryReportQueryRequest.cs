using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport
{
    public class GetMessageDeliveryReportQueryRequest : IRequest<GetMessageDeliveryReportResponse>
    {
        public int? start { get; set; }
        public int? length { get; set; }
        public string? search { get; set; }
        public string OrderDir { get; set; }
        public string? CompanyKey { get; set; }
        public int draw { get; set; }
    }
}
