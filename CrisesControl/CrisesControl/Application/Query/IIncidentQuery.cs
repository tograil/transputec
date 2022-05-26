using CrisesControl.Api.Application.Query.Common;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Paging;

namespace CrisesControl.Api.Application.Query
{
    public interface IIncidentQuery
    {
        List<IncidentList> GetAllCompanyIncident(int userId);
        List<IncidentTypeReturn> GetCompanyIncidentType(int companyId);
        List<AffectedLocation> GetAffectedLocations(int companyId, string locationType);
        List<AffectedLocation> GetIncidentLocations(int companyId, int incidentActivationId);
        List<CommsMethods> GetIncidentComms(int itemID, string type);
        IncidentDetails GetCompanyIncidentById(int companyId, int incidentId, string userStatus);
        DataTablePaging GetAllActiveCompanyIncident(string? status, PagedRequest pagedRequest);
    }
}
