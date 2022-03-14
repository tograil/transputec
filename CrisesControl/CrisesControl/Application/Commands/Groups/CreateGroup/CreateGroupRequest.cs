using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupRequest : IRequest<CreateGroupResponse>
    {
        public int GroupId { get; set; }  
        public int CompanyId { get; set; }
        public string? GroupName { get; set; }
    }
}
