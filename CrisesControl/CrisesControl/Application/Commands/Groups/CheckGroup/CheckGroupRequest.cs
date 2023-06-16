using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CheckGroup
{
    public class CheckGroupRequest:IRequest<CheckGroupResponse>
    {
        public string GroupName { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
    }
}
