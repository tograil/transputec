using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.TestTemplate
{
    public class TestTemplateRequest:IRequest<TestTemplateResponse>
    {
        public string EmailContent { get; set; }
        public string EmailSubject { get; set; }
        public List<string> ExtraEmailList { get; set; }
    }
}
