using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CrisesControlContext _context;

    public CompanyRepository(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllCompanies()
    {
        return await _context.Set<Company>().AsNoTracking().ToArrayAsync();
    }

    public async Task<IEnumerable<Company>> GetAllCompanyList(int? status, string? companyProfile)
    {
        var currentStatus = status ?? -1;
        var currentCompanyProfile = companyProfile ?? string.Empty;

        var companies = await _context.Set<Company>()
            .Include(x => x.Users)
            .Include(x => x.PackagePlan)
            .Include(x => x.CompanyPaymentProfiles)
            .Where(x => 
                (currentStatus > 0 && currentCompanyProfile == "ON_TRIAL" && x.OnTrial && x.Status == currentStatus)
                || (currentStatus > 0 && currentCompanyProfile == "ALL" && x.Users.Any(y => y.RegisteredUser) && x.Status == currentStatus)
                || (currentStatus > 0 && currentCompanyProfile == "" && x.Users.Any(y => y.RegisteredUser) && x.Status == currentStatus)
                || (currentStatus <= 0 && currentCompanyProfile == "ON_TRIAL" && x.Users.Any(y => y.RegisteredUser) && x.OnTrial && x.CompanyProfile == currentCompanyProfile)
                || (currentStatus <= 0 && currentCompanyProfile != "ALL" && x.Users.Any(y => y.RegisteredUser) && x.CompanyProfile == currentCompanyProfile)
                || (currentStatus <= 0 && currentCompanyProfile == "ALL" && x.Users.Any(y => y.RegisteredUser)))
            .AsNoTracking()
            .ToArrayAsync();

        return companies;
    }
}