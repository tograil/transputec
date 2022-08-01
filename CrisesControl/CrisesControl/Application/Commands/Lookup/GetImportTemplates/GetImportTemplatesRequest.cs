using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates
{
    public class GetImportTemplatesRequest:IRequest<GetImportTemplatesResponse>
    {
        public string Type { get; set; }
    }
}
