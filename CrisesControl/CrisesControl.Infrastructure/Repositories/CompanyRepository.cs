using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CrisesControlContext _context;
    private readonly IGlobalParametersRepository _globalParametersRepository;

    public CompanyRepository(CrisesControlContext context, IGlobalParametersRepository globalParametersRepository)
    {
        _context = context;
        _globalParametersRepository = globalParametersRepository;
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
            .Include(x => x.Users.Where(u => u.UserRole == "SUPERADMIN"))
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

    public async Task<string> GetCompanyParameter(string key, int companyId, string @default = "",
        string customerId = "")
    {

        key = key.ToUpper();

        if (companyId > 0)
        {
            var lkp = await _context.Set<CompanyParameter>()
                .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == companyId);
            
            if (lkp != null)
            {
                @default = lkp.Value;
            }
            else
            {
                var lpr = await _context.Set<LibCompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key);

                @default = lpr != null ? lpr.Value : _globalParametersRepository.LookupWithKey(key, @default);
            }
        }

        if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key))
        {
            var cmp = await _context.Set<Company>().FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (cmp != null)
            {
                var lkp = await _context.Set<CompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == cmp.CompanyId);
                if (lkp != null)
                {
                    @default = lkp.Value;
                }
            }
            else
            {
                @default = "NOT_EXIST";
            }
        }

        return @default;
    }

    public async Task<int> CreateCompany(Company company, CancellationToken token)
    {
        await _context.AddAsync(company, token);

        await _context.SaveChangesAsync(token);

        return company.CompanyId;
    }
}