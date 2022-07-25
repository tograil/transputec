using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList
{
    public class GetActiveTaskCheckListResponse
    {
        public List<ActiveCheckList> Data { get; set; }
    }
}
