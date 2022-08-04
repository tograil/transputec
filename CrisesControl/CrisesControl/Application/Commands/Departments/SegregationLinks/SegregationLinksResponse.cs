using CrisesControl.Core.Groups;

namespace CrisesControl.Api.Application.Commands.Departments.SegregationLinks
{
    public class SegregationLinksResponse
    {
        public List<GroupLink> Data { get; set; }
        public string Message { get; set; }
    }
}
