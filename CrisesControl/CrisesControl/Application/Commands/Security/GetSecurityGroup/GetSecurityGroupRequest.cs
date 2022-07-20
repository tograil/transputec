using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetSecurityGroup
{
    public class GetSecurityGroupRequest:IRequest<GetSecurityGroupResponse>
    {
        public int SecurityGroupId { get; set; }
    }
}
