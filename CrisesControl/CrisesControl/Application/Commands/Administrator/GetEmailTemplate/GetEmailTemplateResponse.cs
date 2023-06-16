using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate
{
    public class GetEmailTemplateResponse
    {
        public List<EmailTemplateList> Data { get; set; }
        public string Message { get; set; }
    }
}
