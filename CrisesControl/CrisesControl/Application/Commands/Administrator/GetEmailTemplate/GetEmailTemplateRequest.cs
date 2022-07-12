using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailTemplate
{
    public class GetEmailTemplateRequest:IRequest<GetEmailTemplateResponse>
    {
        public int TemplateID { get; set; }           
        public string Code { get; set; }              
        public string Locale { get; set; }
        public int Status { get; set; }
        public int QCompanyID { get; set; }
    }
}
