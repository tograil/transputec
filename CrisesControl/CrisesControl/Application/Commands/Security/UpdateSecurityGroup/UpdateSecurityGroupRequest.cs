using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup
{
    public class UpdateSecurityGroupRequest:IRequest<UpdateSecurityGroupResponse>
    {
        public int SecurityGroupId { get; set; }
        
        [MaxLength(150)]
        public string GroupName { get; set; }
        [MaxLength(500)]
        public string GroupDescription { get; set; }
        public int[] GroupSecurityObjects { get; set; }
        [MaxLength(20)]
        public string UserRole { get; set; }
        public int Status { get; set; }
    }
}
