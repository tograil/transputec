using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;

namespace CrisesControl.Api.Application.Query
{
    public interface IAdminQuery
    {
        Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request);
    }
}
