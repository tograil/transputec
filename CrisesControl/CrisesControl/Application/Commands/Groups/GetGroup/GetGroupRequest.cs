using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupRequest : IRequest<GetGroupResponse>
    {
        public int GroupId { get; set; }
    }
}
