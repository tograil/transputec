using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType;

namespace CrisesControl.Api.Application.Query
{
    public interface IAdminQuery
    {
        Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request);
        Task<DumpReportResponse> DumpReport(DumpReportRequest request);
        Task<GetReportResponse> GetReport(GetReportRequest request);
        Task<AddLibIncidentResponse> AddLibIncident(AddLibIncidentRequest request);
        Task<UpdateLibIncidentResponse> UpdateLibIncident(UpdateLibIncidentRequest request);
        Task<DeleteLibIncidentResponse> DeleteLibIncident(DeleteLibIncidentRequest request);
        Task<GetLibIncidentResponse> GetLibIncident(GetLibIncidentRequest request);
        Task<GetAllLibIncidentTypeResponse> GetAllLibIncidentType(GetAllLibIncidentTypeRequest request);
        Task<UpdateLibIncidentTypeResponse> UpdateLibIncidentType(UpdateLibIncidentTypeRequest request);
        Task<GetLibIncidentTypeResponse> GetLibIncidentType(GetLibIncidentTypeRequest request);
        Task<AddLibIncidentTypeResponse> AddLibIncidentType(AddLibIncidentTypeRequest request);
        Task<DeleteLibIncidentTypeResponse> DeleteLibIncidentType(DeleteLibIncidentTypeRequest request);
        Task<GetCompanyPackageFeaturesResponse> GetCompanyPackageFeatures(GetCompanyPackageFeaturesRequest request);
    }
}
