using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.CompanyAggregate.Services;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CrisesControl.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly CrisesControlContext _context;

    public CompanyService(CrisesControlContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllCompanies()
    {
        return await _context.Set<Company>().AsNoTracking().ToArrayAsync();
    }

    public async Task<IEnumerable<CompanyRoot>> GetAllCompanyList(int? status, string? companyProfile)
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

        return companies.Select(c =>
        {
            var user = c.Users.First(x => x.UserRole == "SUPERADMIN");
            var companyPaymentProfile = c.CompanyPaymentProfiles?.FirstOrDefault();

            return new CompanyRoot
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                PrimaryEmail = user.PrimaryEmail,
                AgreementNo = companyPaymentProfile?.AgreementNo ?? string.Empty,
                CompanyLogo = c.CompanyLogoPath ?? string.Empty,
                CompanyProfile = c.CompanyProfile ?? string.Empty,
                ContractAnniversary = companyPaymentProfile?.ContractAnniversary ?? c.RegistrationDate,
                RegistrationDate = c.RegistrationDate,
                CustomerId = c.CustomerId ?? string.Empty,
                InvitationCode = c.InvitationCode ?? string.Empty,
                IsdCode = c.Isdcode ?? string.Empty,
                MobileNo = c.SwitchBoardPhone ?? string.Empty,
                OnTrial = c.OnTrial,
                PlanName = c.PackagePlan.PlanName,
                Status = c.Status,
                SwitchBoardPhone = c.SwitchBoardPhone ?? string.Empty,
            };
        }).ToArray();
    }
}