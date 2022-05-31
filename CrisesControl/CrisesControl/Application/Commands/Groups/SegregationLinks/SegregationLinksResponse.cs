using CrisesControl.Core.Groups;

namespace CrisesControl.Api.Application.Commands.Groups.SegregationLinks
{
    public class SegregationLinksResponse
    {
        public List<GroupLink> data { get; set; }
        public string Message { get; set; }
    }
}
