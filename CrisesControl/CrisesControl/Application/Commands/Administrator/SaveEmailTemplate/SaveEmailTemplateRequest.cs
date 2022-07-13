using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveEmailTemplate
{
    public class SaveEmailTemplateRequest:IRequest<SaveEmailTemplateResponse>
    {
        public int TemplateID { get; set; }
       
        public string Type { get; set; }
      
        public string Code { get; set; }
        
        public string Name { get; set; }
       
        public string Description { get; set; }
        public string HtmlData { get; set; }
        public string EmailSubject { get; set; }
        
        public string Locale { get; set; }
        public int Status { get; set; }
        public int QCompanyID { get; set; }
    }
}
