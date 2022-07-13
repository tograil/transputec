using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SendCustomerNotice
{
    public class SendCustomerNoticeRequest:IRequest<SendCustomerNoticeResponse>
    {
        public string EmailContent { get; set; }
        public string EmailSubject { get; set; }
        public List<string> ExtraEmailList { get; set; }
    }
}
