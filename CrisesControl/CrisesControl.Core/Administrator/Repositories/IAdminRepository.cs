using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrisesControl.Core.Administrator.Repositories
{
    public interface IAdminRepository
    {
        Task<List<LibIncident>> GetAllLibIncident();
        Task<DataTable> GetReportData(int ReportID, List<ReportParam> sqlParams, string rFilePath, string rFileName);
        Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",");
        Task<AdminResult> GetReportList(int ReportID);
        Task<bool> DeleteLibIncidentType(LibIncidentType libIncidentType);
        Task<bool> DeleteLibIncident(LibIncident libIncident);
        Task<int> AddLibIncident(LibIncident libIncident);
        Task<int> UpdateLibIncident(LibIncident libIncident);
        Task<int> AddLibIncidentType(LibIncidentType libIncidentType);
        Task<int> UpdateLibIncidentType(LibIncidentType libIncidentType);
        Task<LibIncident> GetLibIncidentByName(string Name);
        Task<LibIncidentType> GetLibIncidentTypeByName(string Name);
        Task<AdminLibIncident> GetLibIncident(int LibIncidentId);
        Task<LibIncident> GetLibIncidentById(int LibIncidentId);
        Task<List<LibIncidentType>> GetAllLibIncidentType();
        Task<LibIncidentType> GetLibIncidentType(int LibIncidentTypeId);
        Task<LibIncidentType> GetLibIncidentTypeById(int LibIncidentTypeId);
        Task<List<CompanyPackageFeatureList>> GetCompanyPackageFeatures(int OutUserCompanyId);
        Task<List<CompanyPackageFeatureList>> GetCompanyModules(int OutUserCompanyId);

    }
}
