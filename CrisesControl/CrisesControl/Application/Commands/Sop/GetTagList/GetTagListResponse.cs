using CrisesControl.Core.Sop;

namespace CrisesControl.Api.Application.Commands.Sop.GetTagList
{
    public class GetTagListResponse
    {
        public List<ContentTags> data { get; set; }
        public string Message { get; set; }
    }
}
