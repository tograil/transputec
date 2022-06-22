using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CrisesControl.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CrisesControlContext _context;
    private readonly IGlobalParametersRepository _globalParametersRepository;
    private readonly ILogger<CompanyRepository> _logger;
    private readonly ISenderEmailService _senderEmailService;

    public CompanyRepository(CrisesControlContext context, ISenderEmailService senderEmailService, IGlobalParametersRepository globalParametersRepository, ILogger<CompanyRepository> logger)
    {
        _context = context;
        _globalParametersRepository = globalParametersRepository;
        _logger = logger;
        _senderEmailService = senderEmailService;
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

    public async Task<string> GetTimeZone(int companyId)
    {
        return (await _context.Set<Company>()
            .Include(x => x.StdTimeZone)
            .FirstOrDefaultAsync(x => x.CompanyId == companyId))?.StdTimeZone?.ZoneLabel ?? "GMT Standard Time";
    }
    public async Task<Company> GetCompanyByID(int companyId)        
    {
        return await _context.Set<Company>().Include(x=>x.PackagePlan).FirstOrDefaultAsync(x => x.CompanyId == companyId);
    }
    public async Task<int> UpdateCompanyDRPlan(Company company)
    {
      
        _context.Entry(company).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Updated Company DR Plan {company.CompanyId}");

        return company.CompanyId;
    }

    public async Task<int> UpdateCompanyLogo(Company company)
    {
        _context.Entry(company).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Updated Company logo for {company.CompanyId}");

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

    public async Task<int> UpdateCompany(Company company)
    {
        
          _context.Update(company);
         await _context.SaveChangesAsync();
        _logger.LogInformation($"Company has been updated {company.CompanyId}");
        return company.CompanyId;
            

           
    }
    public async Task<bool> DuplicateCompany(string CompanyName, string Countrycode)
    {
        try
        {
            var chkCompany = await _context.Set<Company>().Include(AL => AL.AddressLink).Include(a => a.AddressLink.Address)
                            .Where(C => C.CompanyName == CompanyName.Trim() && C.AddressLink.Address.CountryCode == Countrycode.Trim())
                             .Select(A => new{A.AddressLink.Address.CountryCode, A.CompanyName }).FirstOrDefaultAsync();
            if (chkCompany != null)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }
    public async Task<dynamic> DeleteCompanyComplete(int CompanyId, int UserId, string GUID, string DeleteType)
    {

        try
        {
            string Message = "";
            var userdata = await _context.Set<User>().Where(U => U.UniqueGuiId == GUID).FirstOrDefaultAsync(); 

            if (userdata != null)
            {
                UserId = userdata.UserId;
                CompanyId = userdata.CompanyId;
                if (DeleteType.ToUpper() == "COMPANY")
                {
                    if (userdata.RegisteredUser == true)
                    {
                        var compDelete = await _context.Set<Company>().Where(C=> C.CompanyId == CompanyId).FirstOrDefaultAsync();
                        if ( compDelete.Status != 1)
                        {

                            UserFullName primaryUserName = new UserFullName { Firstname = userdata.FirstName, Lastname = userdata.LastName };

                            string primaryUserEmail = userdata.PrimaryEmail;
                            PhoneNumber primaryUserMobile = new PhoneNumber { ISD = userdata.Isdcode, Number = userdata.MobileNo };

                            var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                            var response = await _context.Set<Company>().FromSqlRaw("exec SP_Delete_Company_Hard @CompanyId", pCompanyID).FirstOrDefaultAsync();

                            //SendEmail SDE = new SendEmail();
                           await _senderEmailService.RegistrationCancelled(compDelete.CompanyName, (int)compDelete.PackagePlanId, compDelete.RegistrationDate, primaryUserName, primaryUserEmail, primaryUserMobile);

                            try
                            {
                                Task.Factory.StartNew(() => { DeleteCompanyApi(CompanyId, compDelete.CustomerId); });
                            }
                            catch (Exception ex)
                            {
                                throw new CompanyNotFoundException(CompanyId, UserId);
                            }

                            return true;

                        }
                        else
                        {
                                                   
                            Message = "This company is already active and cannot be deleted in this manner. Please login to the portal to cancel your membership.";
                        }
                    }
                    else
                    {
                                           
                        Message = "User is not registered.";
                    }
                }
                else if (DeleteType == "USER")
                {
                    if (userdata.Status != 1)
                    {
                        var DeleteUserSecurityGroup = await _context.Set<UserSecurityGroup>().Where(USG=> USG.UserId == UserId).ToListAsync();
                        _context.RemoveRange(DeleteUserSecurityGroup);
                        await _context.SaveChangesAsync();

                        var Deletecompanycomms = await _context.Set<UserComm>().Where(UC=> UC.UserId == UserId && UC.CompanyId == CompanyId).ToListAsync();
                        _context.RemoveRange(Deletecompanycomms);
                        await _context.SaveChangesAsync();

                        var DelOBjs = (from OJR in _context.Set<ObjectRelation>()
                                       let ObjMap = from OM in _context.Set<ObjectMapping>()
                                                    join OS in _context.Set<CrisesControl.Core.Models.Object>() on OM.SourceObjectId equals OS.ObjectId
                                                    join OT in _context.Set<CrisesControl.Core.Models.Object>() on OM.TargetObjectId equals OT.ObjectId
                                                    where (OS.ObjectName == "GroupDetails" || OS.ObjectName == "LocationDetails")
                                                    && OT.ObjectName == "UserDetails"
                                                    select OM.ObjectMappingId
                                       where OJR.TargetObjectPrimaryId == UserId && ObjMap.Contains(OJR.ObjectMappingId)
                                       select OJR);
                        _context.RemoveRange(DelOBjs);
                        await _context.SaveChangesAsync();

                        _context.Remove(userdata);
                       await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        
                        Message = "This account is already active and cannot be deleted in this manner. Please ask your system administrator to delete your account from Crises Control.";
                    }
                }
            }
            else
            {
                return false;
            }
            return userdata;
        }
        catch (Exception ex)
        {
            throw new CompanyNotFoundException(CompanyId, UserId);
        }
    }
    public async Task<bool> DeleteCompanyApi(int CompanyId, string CustomerId)
    {
        try
        {
            string CompanyApi = await LookupWithKey("COMPANY_API_MGMT_URL");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(CompanyApi);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var cmpapirequest = new CompanyApiRequest()
            {
                ApiHost = "",
                CompanyId = CompanyId,
                CompanyName = "",
                CustomerId = CustomerId,
                InvitationCode = "",
                ApiMode = ""
            };

            HttpResponseMessage RspApi = client.PostAsJsonAsync("Company/DeleteCompanyApi", cmpapirequest).Result;
            Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
            string ressultstr = resultstring.Result.Trim();
            if (RspApi.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                //DBC.CreateLog("INFO", ressultstr, null, "RegisterHelper", "InsertCompanyApi", CompanyId);
                return false;
            }

        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }
    public async Task<string> LookupWithKey(string Key, string Default = "")
    {
        try
        {
            Dictionary<string, string> Globals = CCConstants.GlobalVars;
            if (Globals.ContainsKey(Key))
            {
                return Globals[Key];
            }


            var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
            if (LKP != null)
            {
                Default = LKP.Value;
            }
            return Default;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            return Default;
        }
    }

    public async Task<AddressLink> GetCompanyAddress(int CompanyID)
    {
                var AddressInfo = await _context.Set<AddressLink>().Include(x=>x.Address)
                                   //join AddLink in db.AddressLink on Addressval.AddressId equals AddLink.AddressId
                                  .Where(Addressval=> Addressval.CompanyId == CompanyID && Addressval.Address.AddressType == "Primary"
                                  ).FirstOrDefaultAsync();
        return AddressInfo;
    }

    public async Task<dynamic> GetSite(int SiteID, int CompanyID)
    {
        try
        {
            if (SiteID > 0)
            {
                var site = await _context.Set<Site>().Where(S=> S.SiteId == SiteID && S.CompanyId == CompanyID).FirstOrDefaultAsync();
                return site;
            }
            else
            {
                var sites= await _context.Set<Site>().Where(S=> S.CompanyId == CompanyID).ToListAsync();
                return sites;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<int> SaveSite(Site site)
    {
        await _context.AddAsync(site);

        await _context.SaveChangesAsync();


        _logger.LogInformation($"Added new site {site.SiteId}");

        return site.SiteId;
    }
    public async Task<Site> GetCompanySiteById(int SiteID, int CompanyID)
    {
       var site= await _context.Set<Site>().Where(S=> S.SiteId == SiteID && S.CompanyId == CompanyID ).FirstOrDefaultAsync();
        return site;
    }
    public async Task<GetCompanyDataResponse> GetStarted(int CompanyID)
    {

        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
           var data=await _context.Set<GetCompanyDataResponse>().FromSqlRaw("exec Pro_Company_GetCompanyData @CompanyID", pCompanyID).FirstOrDefaultAsync();
            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<SocialIntegraion>> GetSocialIntegration(int CompanyID, string AccountType)
    {
        try
        {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pAccountType = new SqlParameter("@AccountType", AccountType);

            var result = await _context.Set<SocialIntegraion>().FromSqlRaw("EXEC Pro_Get_Social_Integration @CompanyID, @AccountType", pCompanyID, pAccountType).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
            return null;
        }
    }
}