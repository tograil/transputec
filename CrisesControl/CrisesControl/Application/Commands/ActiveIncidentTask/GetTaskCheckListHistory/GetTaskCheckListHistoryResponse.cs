using CrisesControl.Core.Tasks;
using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory
{
    public class GetTaskCheckListHistoryResponse
    {
        public List<CheckListHistoryRsp> Data { get; set; }
        public string Message { get; set; }
    }
}
