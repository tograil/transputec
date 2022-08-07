using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.AddSecurityGroup
{
    public class AddSecurityGroupRequest:IRequest<AddSecurityGroupResponse>
    {
     
         public string GroupName { get; set; }
     
        public string GroupDescription { get; set; }
        public int[] GroupSecurityObjects { get; set; }
 
        public string UserRole { get; set; }
        public int Status { get; set; }
    }
}
