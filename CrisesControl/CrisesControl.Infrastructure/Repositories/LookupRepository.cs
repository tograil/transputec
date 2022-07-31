using CrisesControl.Core.Companies;
using CrisesControl.Core.Lookup.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class LookupRepository: ILookupRepository
    {
        private readonly CrisesControlContext _context;
        public LookupRepository(CrisesControlContext context)
        {
            this._context = context;
        }

        public async Task<List<AssetType>> AssetTypes()
        {
            var TypeList = await _context.Set<AssetType>().OrderBy(AT=> AT.TypeName).ToListAsync();
            return TypeList;
        }

        public async Task<List<UserDepartment>> GetAllTmpDept()
        {
            var tempDept = await _context.Set<UserDepartment>().ToListAsync();
            return tempDept;
        }

        public async Task<List<UserLocation>> GetAllTmpLoc()
        {
            var tempLoc = await _context.Set<UserLocation>().ToListAsync();
            return tempLoc;
        }

        public async Task<List<Registration>> GetAllTmpUser()
        {
            var TmpUser = await _context.Set<Registration>().ToListAsync();
            return TmpUser;
        }

        public async Task<List<Country>> GetCountries()
        {
            var Country = await _context.Set<Country>().OrderBy(CN=> CN.Name).ToListAsync();
            return Country;
        }

        public async Task<List<Icon>> GetIcons(int companyId)
        {
            var icons = await _context.Set<Icon>().Where(IC=> IC.CompanyId == companyId || IC.CompanyId == 0).OrderBy(o => o.IconTitle).ToListAsync();
            return icons;
        }

        public async Task<List<ImportTemplate>> GetImportTemplates(string type)
        {
            var Templates = await _context.Set<ImportTemplate>().Where(IT=> IT.TemplateType == type ).OrderBy(o => o.TemplateName).ToListAsync();
            return Templates;
        }

        public async Task<UserDepartment> GetTempDept(int id)
        {
            var tempDept = await _context.Set<UserDepartment>().Where(TD => TD.DepartmentId == id).FirstOrDefaultAsync();
            return tempDept;
        }

        public async Task<UserLocation> GetTempLoc(int id)
        {
            var tempLoc = await _context.Set<UserLocation>().Where(TL => TL.LocationId == id).FirstOrDefaultAsync();
            return tempLoc;
        }

        public async Task<Registration> GetTempUser(int Id)
        {
            var register = await _context.Set<Registration>().Where(TL => TL.Id == Id).FirstOrDefaultAsync();
            return register;
        }

        public async Task<List<StdTimeZone>> GetTimeZone()
        {
            var TimeZone = await _context.Set<StdTimeZone>().OrderBy(TZ=> TZ.TimeZoneId).ToListAsync();
            return TimeZone;
        }
    }
}
