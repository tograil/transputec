using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompanyComplete
{
    public class DeleteCompanyCompleteRequest:IRequest<DeleteCompanyCompleteResponse>
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string GUID { get; set; }
        public string DeleteType { get; set; }
    }
}
