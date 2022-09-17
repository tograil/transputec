using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Core.Groups;

namespace CrisesControl.Api.Application.Commands.Groups.GetAllGroup
{
    public class GetAllGroupResponse
    {
        public List<GroupDetail> Data { get; set; }
    }
}
