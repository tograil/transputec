using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpdateCompanyStatus
{
    public class UpdateCompanyStatusRequest : IRequest<UpdateCompanyStatusResponse>
    {
        public string CompanyProfile { get; set; }
        public int Status { get; set; }
        //public int CurrentUserId { get; set; }
        //public int CompanyId { get; set; }
    }
}
