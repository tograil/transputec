using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup
{
    public class UpdateSecurityGroupRequest:IRequest<UpdateSecurityGroupResponse>
    {
        public int SecurityGroupId { get; set; }
        public string GroupName { get; set; }     
        public string GroupDescription { get; set; }
        public int[] GroupSecurityObjects { get; set; }       
        public string UserRole { get; set; }
        public int Status { get; set; }
    }
}
