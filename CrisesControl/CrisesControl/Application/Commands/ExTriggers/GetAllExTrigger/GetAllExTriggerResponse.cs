using CrisesControl.Core.ExTriggers;

namespace CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger
{
    public class GetAllExTriggerResponse
    {
       public List<ExTriggerList> Data { get; set; }
       public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
