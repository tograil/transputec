using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupRequest : IRequest<GetGroupResponse>
    {
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
    }
}
