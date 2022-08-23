using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory
{
    public class GetTaskCheckListHistoryResponse
    {
        public List<CheckListHistoryRsp> Data { get; set; }
        public string Message { get; set; }
    }
}
