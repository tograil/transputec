using CrisesControl.Core.ExTriggers;
using CrisesControl.Core.Models;
namespace CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger
{
    public class GetExTriggerResponse
    {
        public List<ExTriggerList> Data { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
     }
}
