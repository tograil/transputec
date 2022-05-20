using CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger;

namespace CrisesControl.Api.Application.Query
{
    public interface IExTriggerQuery
    {
        Task<GetAllExTriggerResponse> GetAllExTrigger(GetAllExTriggerRequest request);
        Task<GetExTriggerResponse> GetExTrigger(GetExTriggerRequest request);
        Task<GetImpTriggerResponse> GetImpTrigger(GetImpTriggerRequest request);
    }
}
