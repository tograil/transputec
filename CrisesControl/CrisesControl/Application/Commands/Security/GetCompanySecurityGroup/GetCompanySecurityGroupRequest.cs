using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup
{
    public class GetCompanySecurityGroupRequest : IRequest<GetCompanySecurityGroupResponse>
    {
            public int CompanyID { get; set; }
    }
}
