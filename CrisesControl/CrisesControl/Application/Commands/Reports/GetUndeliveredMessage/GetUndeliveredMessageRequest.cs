using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUndeliveredMessage
{
    public class GetUndeliveredMessageRequest:IRequest<GetUndeliveredMessageResponse>
    {
        public int MessageID { get; set; }
        public string CommsMethod { get; set; }
        public string CountryCode { get; set; }
        public string ReportType { get; set; }
        public string CompanyKey { get; set; }
        public string search { get; set; }
        public string orderDir { get; set; }
        public int Draw { get; set; }
    }
}
