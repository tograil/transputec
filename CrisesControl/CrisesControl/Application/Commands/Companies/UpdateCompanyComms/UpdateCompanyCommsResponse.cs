using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyComms
{
    public class UpdateCompanyCommsResponse
    {
        public ReplyChannel Data { get; set; }
        public string Message { get; set; }
    }
}
