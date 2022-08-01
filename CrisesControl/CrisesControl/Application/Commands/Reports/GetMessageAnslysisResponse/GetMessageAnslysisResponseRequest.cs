using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageAnslysisResponse
{
    public class GetMessageAnslysisResponseRequest:IRequest<GetMessageAnslysisResponseResponse>
    {
        public int MessageId { get; set; }
        public MessageType MessageType { get; set; }
        public int DrillOpt { get; set; }
        public string Search { get; set; }
        public string orderDir { get; set; }
        public string CompanyKey { get; set; }
        public int Draw { get; set; }
    }
}
