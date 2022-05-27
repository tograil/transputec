using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport
{
    public class GetMessageDeliveryReportRequest : IRequest<GetMessageDeliveryReportResponse>
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }        
        public int draw { get; set; }       
        public string? search { get; set; }
        public string OrderDir { get; set; }
        public string? CompanyKey { get; set; }
    }
}
