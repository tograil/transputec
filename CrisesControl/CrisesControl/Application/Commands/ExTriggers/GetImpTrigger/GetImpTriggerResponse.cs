using CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger;
using CrisesControl.Core.ExTriggers;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger
{
    public class GetImpTriggerResponse
    {
        public List<ExTriggerList> Data { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
