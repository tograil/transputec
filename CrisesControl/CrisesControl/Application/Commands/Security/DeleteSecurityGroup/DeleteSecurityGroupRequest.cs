using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup
{
    public class DeleteSecurityGroupRequest:IRequest<DeleteSecurityGroupResponse>
    {
        public int SecurityGroupId { get; set; }
    }
}
