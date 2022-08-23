using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.UpdateCompanyStatus
{
    public class UpdateCompanyStatusRequest : CcBase, IRequest<UpdateCompanyStatusResponse>
    {
        public string CompanyProfile { get; set; }
        public int Status { get; set; }
    }
}
