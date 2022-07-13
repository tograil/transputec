using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.RestoreTemplate
{
    public class RestoreTemplateRequest:IRequest<RestoreTemplateResponse>
    {
        public string Code { get; set; }
    }
}
