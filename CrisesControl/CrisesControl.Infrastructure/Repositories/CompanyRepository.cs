﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

    public async Task<CompanyInfoReturn> GetCompany(CompanyRequestInfo company, CancellationToken token)
    {
        CompanyInfoReturn compnayrtn = new CompanyInfoReturn();

        var Companydata = await (from CompanyVal in _context.Set<Company>()
                           where CompanyVal.CompanyId == company.CompanyId
                                 select CompanyVal).FirstOrDefaultAsync();

        var AddressInfo = await (from Addressval in _context.Set<Address>()
                           join AddLink in _context.Set <AddressLink>() on Addressval.AddressId equals AddLink.AddressId
                           where AddLink.CompanyId == company.CompanyId && Addressval.AddressType == "Primary"
                           select Addressval).FirstOrDefaultAsync();

        if (Companydata != null) {
            compnayrtn.Company_Name = Companydata.CompanyName;
            compnayrtn.CompanyProfile = Companydata.CompanyProfile;
            compnayrtn.CompanyLogo = Companydata.CompanyLogoPath;
            compnayrtn.ContactLogo = Companydata.ContactLogoPath;
            compnayrtn.Master_Action_Plan = (Companydata.PlanDrdoc == null || Companydata.PlanDrdoc == "") ? "" : Companydata.PlanDrdoc;
            compnayrtn.Website = (Companydata.Website == null || Companydata.Website == "") ? "" : Companydata.Website;
            compnayrtn.TimeZone = Companydata.TimeZone.ToString();
            compnayrtn.PhoneISDCode = Companydata.Isdcode;
            compnayrtn.SwitchBoardPhone = Companydata.SwitchBoardPhone;
            compnayrtn.AnniversaryDate = Companydata.AnniversaryDate;
            compnayrtn.Fax = Companydata.Fax;
            compnayrtn.OnTrial = Companydata.OnTrial;
            compnayrtn.CustomerId = Companydata.CustomerId;
            compnayrtn.InvitationCode = Companydata.InvitationCode;
        }
        if (AddressInfo != null) {
            compnayrtn.AddressLine1 = AddressInfo.AddressLine1;
            compnayrtn.AddressLine2 = AddressInfo.AddressLine2;
            compnayrtn.City = AddressInfo.City;
            compnayrtn.State = AddressInfo.State;
            compnayrtn.Postcode = AddressInfo.Postcode;
            compnayrtn.CountryCode = AddressInfo.CountryCode;
        }
        compnayrtn.ErrorId = 0;
        compnayrtn.Message = "CompanyView";
        return compnayrtn;
    }

    public async Task<List<CommsMethod>> GetCommsMethod() {
        List<CommsMethod> result = new List<CommsMethod>();
        try {
            var response = await _context.Set<CommsMethod>().FromSqlRaw("exec Pro_Get_Comms_Methods").ToListAsync();
            if (response != null)
                return response;
        } catch (Exception ex) {

        }
        return result;
    }

    public async Task<int> UpdateDepartment(Company company, CancellationToken token)
    {
        try {
            var result = _context.Set<Company>().Where(t => t.CompanyId == company.CompanyId).FirstOrDefault();

            if (result != null)
            {

                result.CompanyName = company.CompanyName;
                result.Status = company.Status;
                result.UpdatedOn = company.UpdatedOn;
                result.UpdatedBy = company.UpdatedBy;
                result.CompanyLogoPath = company.CompanyLogoPath;
                result.CompanyProfile = company.CompanyProfile;
                result.CompanyPaymentProfiles = company.CompanyPaymentProfiles;
                result.PackagePlanId = company.PackagePlanId;
                result.PlanDrdoc = company.PlanDrdoc;
                result.AndroidLogo = company.AndroidLogo;
                result.ContactLogoPath = company.ContactLogoPath;
                result.CustomerId = company.CustomerId;
                result.Fax = company.Fax;
                result.WindowsLogo = company.WindowsLogo;
                result.Website = company.Website;
                result.InvitationCode = company.InvitationCode;
                result.IOslogo = company.IOslogo;
                result.Isdcode = company.Isdcode;
                result.OnTrial = company.OnTrial;
                result.Sector = company.Sector;
                result.SwitchBoardPhone = company.SwitchBoardPhone;
                result.TimeZone = company.TimeZone;
                result.UniqueKey = company.UniqueKey;
                _context.Update(company);
                await _context.SaveChangesAsync();
                return company.CompanyId;
            }

           
        }
        catch (Exception ex) { 
         Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
        return 0;


    }
}