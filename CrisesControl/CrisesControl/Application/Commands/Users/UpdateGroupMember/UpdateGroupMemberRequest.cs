using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateGroupMember
{
    public class UpdateGroupMemberRequest : IRequest<UpdateGroupMemberResponse>
    {
 
        public int TargetID { get; set; }
        public int UserID { get; set; }
        public int ObjMapID { get; set; }
        public string Action { get; set; }
        
        
    }
}
