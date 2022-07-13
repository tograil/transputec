using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate
{
    public class RestoreTemplateResponse
    {
        public bool template { get; set; }
        public List<EmailTemplateList> Data { get; set; }
    }
}
