using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;

namespace CrisesControl.Api.Application.Query
{
    public interface IActiveIncidentQuery
    {
        Task<GetUserTaskResponse> GetUserTasks(GetUserTaskRequest request);
        Task<ActiveIncidentTasksResponse> ActiveIncidentTasks(ActiveIncidentTasksRequest request);
        Task<AcceptTaskResponse> AcceptTask(AcceptTaskRequest request);
        Task<DeclineTaskResponse> DeclineTask(DeclineTaskRequest request);
        Task<CompleteTaskResponse> CompleteTask(CompleteTaskRequest request);
    }
}
