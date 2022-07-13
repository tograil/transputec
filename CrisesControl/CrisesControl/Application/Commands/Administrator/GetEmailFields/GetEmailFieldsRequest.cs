using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailFields
{
    public class GetEmailFieldsRequest:IRequest<GetEmailFieldsResponse>
    {
        public string TemplateCode { get; set; }
        public int FieldType { get; set; }
    }
}
