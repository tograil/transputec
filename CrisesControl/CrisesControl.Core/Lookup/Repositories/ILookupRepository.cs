using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Lookup.Repositories
{
    public interface ILookupRepository
    {
        Task<List<ImportTemplate>> GetImportTemplates(string type);
        Task<List<Icon>> GetIcons(int companyId);
        Task<List<StdTimeZone>> GetTimeZone();
        Task<List<AssetType>> AssetTypes();
        Task<List<Country>> GetCountries();
        Task<List<Registration>> GetAllTmpUser();
        Task<Registration> GetTempUser(int Id);
        Task<List<UserLocation>> GetAllTmpLoc();
        Task<List<UserDepartment>> GetAllTmpDept();
        Task<UserDepartment> GetTempDept(int id);
        Task<UserLocation> GetTempLoc(int id);
    }
}
