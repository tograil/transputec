using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse
{
    public class SaveActiveCheckListResponseRequest:IRequest<SaveActiveCheckListResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public List<CheckListOption> CheckListResponse { get; set; }
    }
}
