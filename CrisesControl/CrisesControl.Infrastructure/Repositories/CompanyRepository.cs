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
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using CCObj = CrisesControl.Core.Models.Object;

namespace CrisesControl.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository {
    private readonly CrisesControlContext _context;
    private readonly IGlobalParametersRepository _globalParametersRepository;
    private readonly ILogger<CompanyRepository> _logger;
    private readonly ISenderEmailService _senderEmailService;
    private readonly IDBCommonRepository _DBC;

    public CompanyRepository(CrisesControlContext context, IDBCommonRepository DBC, ISenderEmailService senderEmailService, IGlobalParametersRepository globalParametersRepository, ILogger<CompanyRepository> logger)
    {
        _context = context;
        _globalParametersRepository = globalParametersRepository;
        _logger = logger;
        _senderEmailService = senderEmailService;
        _DBC = DBC;
     }

    public async Task<IEnumerable<Company>> GetAllCompanies() {
        return await _context.Set<Company>().AsNoTracking().ToArrayAsync();
    }

    public async Task<IEnumerable<Company>> GetAllCompanyList(int? status, string? companyProfile) {
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
        string customerId = "") {

        key = key.ToUpper();

        if (companyId > 0) {
            var lkp = await _context.Set<CompanyParameter>()
                .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == companyId);

            if (lkp != null) {
                @default = lkp.Value;
            } else {
                var lpr = await _context.Set<LibCompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key);

                @default = lpr != null ? lpr.Value : _globalParametersRepository.LookupWithKey(key, @default);
            }
        }

        if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(key)) {
            var cmp = await _context.Set<Company>().FirstOrDefaultAsync(w => w.CustomerId == customerId);
            if (cmp != null) {
                var lkp = await _context.Set<CompanyParameter>()
                    .FirstOrDefaultAsync(x => x.Name == key && x.CompanyId == cmp.CompanyId);
                if (lkp != null) {
                    @default = lkp.Value;
                }
            } else {
                @default = "NOT_EXIST";
            }
        }

        return @default;
    }

    public async Task<int> CreateCompany(Company company, CancellationToken token) {
        await _context.AddAsync(company, token);

        await _context.SaveChangesAsync(token);

        return company.CompanyId;
    }

    public async Task<string> GetTimeZone(int companyId) {
        return (await _context.Set<Company>()
            .Include(x => x.StdTimeZone)
            .FirstOrDefaultAsync(x => x.CompanyId == companyId))?.StdTimeZone?.ZoneLabel ?? "GMT Standard Time";
    }
    public async Task<Company> GetCompanyByID(int companyId) {
        return await _context.Set<Company>().Include(x => x.PackagePlan).FirstOrDefaultAsync(x => x.CompanyId == companyId);
    }
    public async Task<int> UpdateCompanyDRPlan(Company company) {

        _context.Entry(company).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Updated Company DR Plan {company.CompanyId}");

        return company.CompanyId;
    }

    public async Task<int> UpdateCompanyLogo(Company company) {
        _context.Entry(company).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Updated Company logo for {company.CompanyId}");

        return company.CompanyId;
    }

    public async Task<CompanyInfoReturn> GetCompany(CompanyRequestInfo company, CancellationToken token) {
        CompanyInfoReturn compnayrtn = new CompanyInfoReturn();

        var Companydata = await (from CompanyVal in _context.Set<Company>()
                                 where CompanyVal.CompanyId == company.CompanyId
                                 select CompanyVal).FirstOrDefaultAsync();

        var AddressInfo = await (from Addressval in _context.Set<Address>()
                                 join AddLink in _context.Set<AddressLink>() on Addressval.AddressId equals AddLink.AddressId
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

    public async Task<int> UpdateCompany(Company company) {

        _context.Update(company);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Company has been updated {company.CompanyId}");
        return company.CompanyId;



    }
    public async Task<bool> DuplicateCompany(string CompanyName, string Countrycode) {
        try {
            var chkCompany = await _context.Set<Company>().Include(AL => AL.AddressLink).Include(a => a.AddressLink.Address)
                            .Where(C => C.CompanyName == CompanyName.Trim() && C.AddressLink.Address.CountryCode == Countrycode.Trim())
                             .Select(A => new { A.AddressLink.Address.CountryCode, A.CompanyName }).FirstOrDefaultAsync();
            if (chkCompany != null) {
                return true;
            }
            return false;
        } catch (Exception ex) {
            throw ex;
            return false;
        }
    }
    public async Task<string> DeleteCompanyComplete(int CompanyId, int UserId, string GUID, string DeleteType) {

        try {
            string Message = "";
            var userdata = await _context.Set<User>().Where(U => U.UniqueGuiId == GUID).FirstOrDefaultAsync();

            if (userdata != null) {
                UserId = userdata.UserId;
                CompanyId = userdata.CompanyId;
                if (DeleteType.ToUpper() == "COMPANY") {
                    if (userdata.RegisteredUser == true) {
                        var compDelete = await _context.Set<Company>().Where(C => C.CompanyId == CompanyId).FirstOrDefaultAsync();
                        if (compDelete.Status != 1) {

                            UserFullName primaryUserName = new UserFullName { Firstname = userdata.FirstName, Lastname = userdata.LastName };

                            string primaryUserEmail = userdata.PrimaryEmail;
                            PhoneNumber primaryUserMobile = new PhoneNumber { ISD = userdata.Isdcode, Number = userdata.MobileNo };

                            var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                            var response = await _context.Set<Company>().FromSqlRaw("exec SP_Delete_Company_Hard @CompanyId", pCompanyID).FirstOrDefaultAsync();

                            //SendEmail SDE = new SendEmail();
                            await _senderEmailService.RegistrationCancelled(compDelete.CompanyName, (int)compDelete.PackagePlanId, compDelete.RegistrationDate, primaryUserName, primaryUserEmail, primaryUserMobile);

                            try {
                                await Task.Factory.StartNew(async () => { await DeleteCompanyApi(CompanyId, compDelete.CustomerId); });
                            } catch (Exception ex) {
                                throw new CompanyNotFoundException(CompanyId, UserId);
                            }

                            Message = "Company has been deleted";

                        } else {

                            Message = "This company is already active and cannot be deleted in this manner. Please login to the portal to cancel your membership.";
                        }
                    } else {

                        Message = "User is not registered.";
                    }
                } else if (DeleteType == "USER") {
                    if (userdata.Status != 1) {
                        var DeleteUserSecurityGroup = await _context.Set<UserSecurityGroup>().Where(USG => USG.UserId == UserId).ToListAsync();
                        _context.RemoveRange(DeleteUserSecurityGroup);
                        await _context.SaveChangesAsync();

                        var Deletecompanycomms = await _context.Set<UserComm>().Where(UC => UC.UserId == UserId && UC.CompanyId == CompanyId).ToListAsync();
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
                        Message = "User account has been deleted";
                    } else {

                        Message = "This account is already active and cannot be deleted in this manner. Please ask your system administrator to delete your account from Crises Control.";
                    }
                }
            } else {
                Message = "User is not registered.";
            }
            return Message;
        } catch (Exception ex) {
            throw new CompanyNotFoundException(CompanyId, UserId);
        }
    }
    public async Task<bool> DeleteCompanyApi(int CompanyId, string CustomerId) {
        try {
            string CompanyApi = await LookupWithKey("COMPANY_API_MGMT_URL");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(CompanyApi);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var cmpapirequest = new CompanyApiRequest() {
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
            if (RspApi.IsSuccessStatusCode) {
                return true;
            } else {
                //DBC.CreateLog("INFO", ressultstr, null, "RegisterHelper", "InsertCompanyApi", CompanyId);
                return false;
            }

        } catch (Exception ex) {
            throw ex;
            return false;
        }
    }
    public async Task<string> LookupWithKey(string Key, string Default = "") {
        try {
            Dictionary<string, string> Globals = CCConstants.GlobalVars;
            if (Globals.ContainsKey(Key)) {
                return Globals[Key];
            }


            var LKP = await _context.Set<SysParameter>().Where(w => w.Name == Key).FirstOrDefaultAsync();
            if (LKP != null) {
                Default = LKP.Value;
            }
            return Default;
        } catch (Exception ex) {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            return Default;
        }
    }

    public async Task<AddressLink> GetCompanyAddress(int CompanyID) {
        var AddressInfo = await _context.Set<AddressLink>().Include(x => x.Address)
                          //join AddLink in db.AddressLink on Addressval.AddressId equals AddLink.AddressId
                          .Where(Addressval => Addressval.CompanyId == CompanyID && Addressval.Address.AddressType == "Primary"
                          ).FirstOrDefaultAsync();
        return AddressInfo;
    }

    public async Task<Site> GetSite(int SiteID, int CompanyID) {
        try {

            var site = await _context.Set<Site>().Where(S => S.SiteId == SiteID && S.CompanyId == CompanyID).FirstOrDefaultAsync();
            return site;

        } catch (Exception ex) {
            throw ex;
        }
    }

    public async Task<int> SaveSite(Site site) {
        await _context.AddAsync(site);

        await _context.SaveChangesAsync();


        _logger.LogInformation($"Added new site {site.SiteId}");

        return site.SiteId;
    }
    public async Task<Site> GetCompanySiteById(int SiteID, int CompanyID) {
        var site = await _context.Set<Site>().Where(S => S.SiteId == SiteID && S.CompanyId == CompanyID).FirstOrDefaultAsync();
        return site;
    }
    public async Task<GetCompanyDataResponse> GetStarted(int CompanyID) {

        try {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var data = await _context.Set<GetCompanyDataResponse>().FromSqlRaw("exec Pro_Company_GetCompanyData @CompanyID", pCompanyID).FirstOrDefaultAsync();
            return data;
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<List<SocialIntegraion>> GetSocialIntegration(int CompanyID, string AccountType) {
        try {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pAccountType = new SqlParameter("@AccountType", AccountType);

            var result = await _context.Set<SocialIntegraion>().FromSqlRaw("EXEC Pro_Get_Social_Integration @CompanyID, @AccountType", pCompanyID, pAccountType).ToListAsync();
            return result;
        } catch (Exception ex) {
            throw ex;
            return null;
        }
    }
    public async Task<bool> SaveSocialIntegration(string AccountName, string AccountType, string AuthSecret, string AdnlKeyOne, string AuthToken, string AdnlKeyTwo, int CompanyId, string TimeZoneId, int userId) {
        try {

            var pCompanyID = new SqlParameter("@CompanyID", CompanyId);
            var pAccountName = new SqlParameter("@AccountName", AccountName);
            var pAccountType = new SqlParameter("@AccountType", AccountType);
            var pAuthToken = new SqlParameter("@AuthToken", AuthToken);
            var pAuthSecret = new SqlParameter("@AuthSecret", AuthSecret);
            var pAdnlKeyOne = new SqlParameter("@AdnlKeyOne", AdnlKeyOne);
            var pAdnlKeyTwo = new SqlParameter("@AdnlKeyTwo", AdnlKeyTwo);
            DateTimeOffset CreatedNow = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            var pCreatedNow = new SqlParameter("@UpdatedOn", CreatedNow);
            var pUpdatedBy = new SqlParameter("@UpdatedBy", userId);

            _context.Set<SocialIntegraion>().FromSqlRaw(" exec Pro_Save_Social_Integration @CompanyID, @AccountName, @AccountType, @AuthToken, @AuthSecret, @AdnlKeyOne, @AdnlKeyTwo, @UpdatedOn, @UpdatedBy",
                pCompanyID, pAccountName, pAccountType, pAuthToken, pAuthSecret, pAdnlKeyOne, pAdnlKeyTwo, pCreatedNow, pUpdatedBy);
            return true;
        } catch (Exception ex) {
            throw ex;
            return false;
        }
    }
    public async Task<CompanyCommunication> GetCompanyComms(int CompanyID, int UserID) {

        try {
            var t_bill_users = await GetCompanyParameter("BILLING_USERS", CompanyID);
            var billUsers = new object();

            if (!string.IsNullOrEmpty(t_bill_users)) {
                List<int> uids = t_bill_users.Split(',').Select(int.Parse).ToList();
                billUsers = await _context.Set<User>()
                             .Where(U => uids.Contains(U.UserId) && U.Status == 1)
                             .Select(U => new { U.UserId, UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName } }).ToListAsync();
            }

            var URole = await _context.Set<User>().Where(w => w.UserId == UserID).Select(s => s.UserRole).FirstOrDefaultAsync();
            string HasLowBalance = await CheckFunds(CompanyID, URole);
            var ObjectInfo = await _context.Set<CompanyComm>().Join(_context.Set<CommsMethod>(), c => c.MethodId, cm => cm.CommsMethodId, (c, cm) => new { c, cm })
                            .Where(c => c.c.CompanyId == CompanyID)
                            .Select(CC => new CommsObjects {
                                MethodId = CC.c.MethodId,
                                ServiceStatus = CC.c.ServiceStatus,
                                Status = CC.c.Status,
                                MethodName = CC.cm.MethodName
                            }).ToListAsync();
            var Priority = await _context.Set<PriorityInterval>().Where(PR => PR.CompanyId == CompanyID).OrderBy(o => o.MessageType).ThenBy(t => t.Priority).ToListAsync();

            var PriorityMethod = await _context.Set<PriorityMethod>().Where(PM => PM.CompanyId == CompanyID).OrderBy(PM => PM.PriorityLevel).ToListAsync();
            CompanyCommunication comms = new CompanyCommunication();

            comms.BillUsers = billUsers;
            comms.HasLowBalance = HasLowBalance;
            comms.ObjectInfo = ObjectInfo;
            comms.Priority = Priority;
            comms.PriorityMethod = PriorityMethod;

            return comms;

        } catch (Exception ex) {
            throw ex;
        }
    }

    public async Task<ReplyChannel> GetReplyChannel(int CompanyID, int UserID) {
        try {
            var t_reply_channel = await GetCompanyParameter("DEFAULT_REPLY_CHANNEL", CompanyID);
            //var ObjectInfo = new object();

            ReplyChannel channel = new ReplyChannel();

            if (!string.IsNullOrEmpty(t_reply_channel)) {
                List<int> methodid = t_reply_channel.Split(',').Select(int.Parse).ToList();

                var ObjectInfo = await _context.Set<CompanyComm>().Include(CC => CC.CommsMethod)
                               .Where(CC => CC.CompanyId == CompanyID && methodid.Contains(CC.MethodId)).Select(CC => new {
                                   CC.MethodId,
                                   CC.ServiceStatus,
                                   CC.Status,
                                   CC.CommsMethod.MethodName
                               }).FirstOrDefaultAsync();

                channel.Status = ObjectInfo.Status;
                channel.MethodId = ObjectInfo.MethodId;
                channel.MethodName = ObjectInfo.MethodName;
                channel.ServiceStatus = ObjectInfo.ServiceStatus;



            }

            var URole = await _context.Set<User>().Where(w => w.UserId == UserID).Select(s => s.UserRole).FirstOrDefaultAsync();
            string HasLowBalance = await CheckFunds(CompanyID, URole);
            channel.HasLowBalance = HasLowBalance;




            return channel;

        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<string> CheckFunds(int CompanyID, string UserRole) {
        string ErrorCode = "";
        try {
            string WarningRole = await GetCompanyParameter("SHOW_LOW_BALANCE_WARNING_TO", CompanyID);

            if (UserRole == "USER" && WarningRole == "KEYHOLDER") {
                return ErrorCode;
            }

            var cpp = await _context.Set<CompanyPaymentProfile>().Where(CP => CP.CompanyId == CompanyID).FirstOrDefaultAsync();
            if (cpp != null) {
                if (cpp.CreditBalance < -cpp.CreditLimit) {
                    ErrorCode = "E1001";
                } else if ((cpp.CreditBalance < 0 && cpp.CreditBalance >= (-cpp.CreditLimit)) ||
                    (cpp.CreditBalance < cpp.MinimumBalance)) {
                    ErrorCode = "E1002";
                }
            }
            return ErrorCode;
        } catch (Exception ex) {
            throw ex;
        }

    }
    public async Task<CompanyAccount> GetCompanyAccount(int CompanyID) {
        try {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var companyDetails = await _context.Set<CompanyAccount>().FromSqlRaw("exec Pro_Company_GetCompanyAccount @CompanyID", pCompanyID).FirstOrDefaultAsync();

            if (companyDetails != null) {
                var pCompanyID2 = new SqlParameter("@CompanyID", CompanyID);
                companyDetails.ContractOffer = await _context.Set<PreContractOffer>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_ContractOffer @CompanyID", pCompanyID2).FirstOrDefaultAsync();

                var pCompanyID3 = new SqlParameter("@CompanyID", CompanyID);
                companyDetails.TransactionTypes = await _context.Set<CompanyTransactionType>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_TransactionTypes @CompanyID", pCompanyID3).ToListAsync();

                var pCompanyID4 = new SqlParameter("@CompanyID", CompanyID);
                companyDetails.ActivationKey = await _context.Set<CompanyActivation>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_Activation @CompanyID", pCompanyID4).FirstOrDefaultAsync();

                var pCompanyID5 = new SqlParameter("@CompanyID", CompanyID);
                companyDetails.PaymentProfile = await _context.Set<CompanyPaymentProfile>().FromSqlRaw(" exec Pro_Company_GetCompanyAccount_PaymentProfile @CompanyID", pCompanyID5).FirstOrDefaultAsync();

                var pCompanyID6 = new SqlParameter("@CompanyID", CompanyID);
                companyDetails.PackageItems = await _context.Set<CompanyPackageItem>().FromSqlRaw(" exec Pro_Company_GetCompanyAccount_PackageItems @CompanyID", pCompanyID6).ToListAsync();
            }

            return companyDetails;

        } catch (Exception ex) {
            throw ex;
        }

    }
    public async Task<ReplyChannel> UpdateCompanyComms(int CompanyID, int[] MethodId, int[] BillingUsers, int CurrentAdmin, int CurrentUserID, string TimeZoneId, string Source = "PORTAL") {
        try {
            if (MethodId.Count() > 0) {

                var company = (from CP in _context.Set<CompanyPaymentProfile>()
                               where CP.CompanyId == CompanyID
                               select CP).FirstOrDefault();


                var DelCommList = (from CC in _context.Set<CompanyComm>()
                                   where CC.CompanyId == CompanyID
                                   select CC).ToList();

                List<int> CILIst = new List<int>();
                foreach (int NewMethodId in MethodId) {
                    var ISExist = DelCommList.FirstOrDefault(s => s.MethodId == NewMethodId && s.CompanyId == CompanyID);
                    if (ISExist == null) {
                        CompanyComm CompanyComms = new CompanyComm() {
                            MethodId = NewMethodId,
                            CompanyId = CompanyID,
                            Status = 1,
                            ServiceStatus = true,
                            CreatedBy = CurrentUserID,
                            UpdatedBy = CurrentUserID,
                            UpdatedOn = DateTimeOffset.Now,
                            CreatedOn = DateTimeOffset.Now
                        };
                        _context.Add(CompanyComms);
                    } else {
                        CILIst.Add(ISExist.CompanyCommsId);
                    }
                }

                foreach (var Ditem in DelCommList) {
                    bool ISDEL = CILIst.Any(s => s == Ditem.CompanyCommsId);
                    if (!ISDEL) {
                        Ditem.Status = 3;
                    } else {
                        Ditem.Status = 1;
                    }
                }
                _context.SaveChanges();

                //Delete from the UserComms table - all that are not in the company comms 
                var MethodRemoved = (from CM in _context.Set<CommsMethod>() select CM.CommsMethodId).Except(MethodId).ToList();
                foreach (int RmId in MethodRemoved) {
                    UpdateUserComms(CompanyID, RmId, 0);
                    if (Source == "ADMIN") {
                        var rmc = _context.Set<CompanyComm>().Where(w => w.MethodId == RmId && w.CompanyId == CompanyID).FirstOrDefault();
                        if (rmc != null) {
                            _context.Remove(rmc);
                        }
                    }
                }
                _context.SaveChanges();

                //Add back the method ids to the user for enabled one.
                foreach (int ActiveMethod in MethodId) {
                    UpdateUserComms(CompanyID, ActiveMethod, 1);
                }
            }

            if (BillingUsers.Count() > 0) {
                var CompanyParameter = (from CP in _context.Set<CompanyParameter>()
                                        where CP.CompanyId == CompanyID && CP.Name == "BILLING_USERS"
                                        select CP).FirstOrDefault();
                string joined_list = string.Join(",", BillingUsers);

                if (CompanyParameter != null) {
                    CompanyParameter.Value = joined_list;
                    _context.SaveChanges();
                } else {

                    await AddCompanyParameter("BILLING_USERS", joined_list, CompanyID, CurrentUserID, TimeZoneId);
                }
            }

            if (Source == "ADMIN") {
                var curadmin = await _context.Set<User>().Where(U => U.RegisteredUser == true && U.CompanyId == CompanyID && U.UserId != CurrentAdmin).FirstOrDefaultAsync();
                if (curadmin != null) {
                    var newadmin = await _context.Set<User>().Where(U => U.UserId == CurrentAdmin && U.CompanyId == CompanyID).FirstOrDefaultAsync();
                    if (newadmin != null) {
                        curadmin.RegisteredUser = false;
                        curadmin.UserRole = "ADMIN";
                        newadmin.RegisteredUser = true;
                        newadmin.UserRole = "SUPERADMIN";
                        _context.SaveChanges();
                    }
                }
            }

            var ObjectInfo = await _context.Set<CompanyComm>().Include(CM => CM.CommsMethod)
                              .Where(CC => CC.CompanyId == CompanyID)
                              .Select(CC => new {
                                  CC.MethodId,
                                  CC.ServiceStatus,
                                  CC.Status,
                                  CC.CommsMethod.MethodName
                              }).FirstOrDefaultAsync();

            string bill_users = await GetCompanyParameter("BILLING_USERS", CompanyID);
            ReplyChannel replyChannel = new ReplyChannel();

            replyChannel.Status = ObjectInfo.Status;
            replyChannel.MethodId = ObjectInfo.MethodId;
            replyChannel.MethodName = ObjectInfo.MethodName;
            replyChannel.ServiceStatus = ObjectInfo.ServiceStatus;
            replyChannel.HasLowBalance = bill_users;
            return replyChannel;


        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<bool> CompanyDataReset(string[] ResetOptions, int CompanyID, string TimeZoneId) {
        try {
            foreach (string option in ResetOptions) {
                if (option.ToUpper() == "PINGS") {
                    await ResetPings(CompanyID);
                } else if (option.ToUpper() == "ACTIVEINCIDENT") {
                    await ResetActiveIncident(CompanyID);
                } else if (option.ToUpper() == "GLOBALCONFIG") {
                    await ResetGlobalConfig(CompanyID, TimeZoneId);
                }
                return true;
            }
        } catch (Exception ex) {
            throw ex;
        }
        return false;
    }

    public async Task ResetGlobalConfig(int CompanyID, string TimeZoneId) {
        try {
            DateTimeOffset CreatedNow = DateTime.Now.GetDateTimeOffset(TimeZoneId);
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var pCreatedNow = new SqlParameter("@CreatedOnOffset", CreatedNow);
            await _context.Set<Result>().FromSqlRaw("exec Pro_DC_Global_Config @CompanyID, @CreatedOnOffset", pCompanyID, pCreatedNow).FirstOrDefaultAsync();
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task ResetPings(int CompanyID) {
        try {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            await _context.Set<Result>().FromSqlRaw("exec Pro_DC_Ping @CompanyID", pCompanyID).FirstOrDefaultAsync();
        } catch (Exception ex) {
            throw ex;
        }
    }

    public async Task ResetActiveIncident(int CompanyID) {
        try {
            var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            await _context.Set<Result>().FromSqlRaw("exec Pro_DC_Active_Incident @CompanyID", pCompanyID).FirstOrDefaultAsync();
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<int> DeactivateCompany(Company company) {

        try {

            _context.Update(company);
            await _context.SaveChangesAsync();
            return company.CompanyId;

        } catch (Exception ex) {
            throw ex;
        }
    }

    public async Task<int> ReactivateCompany(Company company) {
        try {
            _context.Update(company);
            await _context.SaveChangesAsync();
            return company.CompanyId;
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<int> AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId) {
        try {
            var comp_param = await _context.Set<CompanyParameter>().Where(CP => CP.CompanyId == CompanyId && CP.Name == Name).AnyAsync();
            if (!comp_param) {
                CompanyParameter NewCompanyParameters = new CompanyParameter() {
                    CompanyId = CompanyId,
                    Name = Name,
                    Value = Value,
                    Status = 1,
                    CreatedBy = CurrentUserId,
                    UpdatedBy = CurrentUserId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                };
                await _context.AddAsync(NewCompanyParameters);
                await _context.SaveChangesAsync();
                return NewCompanyParameters.CompanyParametersId;
            }
        } catch (Exception ex) {
            throw ex;
        }
        return 0;
    }
    private async void UpdateUserComms(int companyId, int userId, int createdUpdatedBy, string timeZoneId = "GMT Standard Time", string pingMethods = "", string incidentMethods = "", bool isNewUser = false, CancellationToken cancellationToken = default) {
        try {

            List<string> ImpPingMethods = pingMethods.Split(',').Select(p => p.Trim().ToUpper()).ToList();
            List<string> ImpInciMethods = incidentMethods.Split(',').Select(p => p.Trim().ToUpper()).ToList();

            if (isNewUser && (ImpPingMethods.Count <= 0 || ImpInciMethods.Count <= 0)) {
                var comms = await (from CC in _context.Set<CompanyComm>()
                                   join CM in _context.Set<CommsMethod>() on CC.MethodId equals CM.CommsMethodId
                                   where CC.CompanyId == companyId
                                   select CM.MethodCode).ToListAsync();
                if (ImpPingMethods.Count <= 0) {
                    ImpPingMethods = comms.ToList();
                }

                if (ImpInciMethods.Count <= 0) {
                    ImpInciMethods = comms.ToList();
                }
            }

            if (ImpPingMethods.Count > 0) {
                ImportUsercomms(companyId, "Ping", userId, ImpPingMethods, createdUpdatedBy, timeZoneId, pingMethods, cancellationToken);
            }

            if (ImpInciMethods.Count > 0) {
                ImportUsercomms(companyId, "Incident", userId, ImpInciMethods, createdUpdatedBy, timeZoneId, incidentMethods, cancellationToken);
            }

        } catch (Exception ex) {
            throw new UserNotFoundException(companyId, userId);
        }
    }
    public async void ImportUsercomms(int companyId, string messageType, int userId, List<string> methodList, int createdUpdatedBy, string timeZoneId, string rawMethodsList, CancellationToken cancellationToken) {
        try {
            var CompanyCommsMethodid = await (from CM in _context.Set<CompanyComm>()
                                              join CP in _context.Set<CommsMethod>() on CM.MethodId equals CP.CommsMethodId
                                              where CM.CompanyId == companyId
                                              select new { CM.MethodId, CP.MethodCode }).ToListAsync();

            var UserMethods = (from UC in _context.Set<UserComm>()
                               join CP in _context.Set<CommsMethod>() on UC.MethodId equals CP.CommsMethodId
                               where UC.UserId == userId
                               && UC.CompanyId == companyId
                               select new { UC, CP }).ToList();

            //Check and assign the default company methods when no ping/incident method are assigned to user
            var incMethods = (from PM in UserMethods where PM.UC.MessageType == messageType select PM).ToList();
            if (incMethods.Count <= 0 && (methodList.Count <= 0 && !string.IsNullOrEmpty(rawMethodsList))) {
                foreach (var comMethod in CompanyCommsMethodid) {
                    CreateUserComms(userId, companyId, comMethod.MethodId, createdUpdatedBy, timeZoneId, messageType);
                }
            } else if (methodList.Count > 0 && !string.IsNullOrEmpty(rawMethodsList)) {
                if (incMethods.Count > 0) {
                    _context.Set<UserComm>().RemoveRange(incMethods.Select(s => s.UC).ToList());
                    await _context.SaveChangesAsync(cancellationToken);
                }
                var method = (from m in CompanyCommsMethodid where methodList.Contains(m.MethodCode) select m).ToList();
                foreach (var comMethod in method) {
                    CreateUserComms(userId, companyId, comMethod.MethodId, createdUpdatedBy, timeZoneId, messageType);
                }
            }

        } catch (Exception ex) {
            throw new UserNotFoundException(companyId, userId);
        }
    }
    public async void CreateUserComms(int UserId, int CompanyId, int MethodId, int CreatedUpdatedBy, string TimeZoneId, string CommType) {
        try {
            var IsCommExist = await _context.Set<UserComm>().Where(UCMM => UCMM.UserId == UserId && UCMM.CompanyId == CompanyId
                             && UCMM.MethodId == MethodId
                            && UCMM.MessageType == CommType).FirstOrDefaultAsync();
            if (IsCommExist == null) {
                UserComm NewUserComms = new UserComm() {
                    UserId = UserId,
                    CompanyId = CompanyId,
                    MethodId = MethodId,
                    MessageType = CommType,
                    Status = 1,
                    Priority = 1,
                    CreatedBy = CreatedUpdatedBy,
                    UpdatedBy = CreatedUpdatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId)
                };
                await _context.AddAsync(NewUserComms);
                await _context.SaveChangesAsync();
            } else {
                IsCommExist.Status = 1;
                await _context.SaveChangesAsync();
            }
        } catch (Exception ex) {
            throw ex;
        }
    }

    public async Task<List<Site>> GetSites(int CompanyID) {
        try {

            var sites = await _context.Set<Site>().Where(S => S.CompanyId == CompanyID).ToListAsync();
            return sites;

        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<string> DeleteCompany(int TargetCompanyID, int CompanyID, int CurrentUserID, string TimeZoneId) {

        try {
            string Message = "";
            var CompanyToDelete = await _context.Set<Company>().Where(C => C.CompanyId == TargetCompanyID).FirstOrDefaultAsync();
            if (CompanyToDelete != null) {
                CompanyToDelete.Status = 3;
                CompanyToDelete.CompanyProfile = "MEMBERSHIP_CANCELLED";
                CompanyToDelete.UpdatedBy = CurrentUserID;
                CompanyToDelete.UpdatedOn = await  _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                await _context.SaveChangesAsync();

                var AllUserOfDeletedCompany = await _context.Set<User>().Where(U => U.CompanyId == TargetCompanyID).ToListAsync();
                UserFullName primaryUserName = AllUserOfDeletedCompany.Where(w => w.RegisteredUser == true).Select(s => new UserFullName { Firstname = s.FirstName, Lastname = s.LastName }).FirstOrDefault();
                string primaryUserEmail = AllUserOfDeletedCompany.Where(w => w.RegisteredUser == true).Select(s => s.PrimaryEmail).FirstOrDefault();
                PhoneNumber primaryUserMobile = AllUserOfDeletedCompany.Where(w => w.RegisteredUser == true).Select(s => new PhoneNumber { ISD = s.Isdcode, Number = s.MobileNo }).FirstOrDefault();

                foreach (var item in AllUserOfDeletedCompany) {
                    if (item.Status != 3) {
                        item.PrimaryEmail = "CompanyDel-" + item.PrimaryEmail;
                    }
                    item.Status = 3;
                    //item.TOKEN = "";
                    await _context.SaveChangesAsync();
                }
               
               await _senderEmailService.RegistrationCancelled(CompanyToDelete.CompanyName, (int)CompanyToDelete.PackagePlanId, CompanyToDelete.RegistrationDate, primaryUserName, primaryUserEmail, primaryUserMobile);

                try {
                    await Task.Factory.StartNew(async () => { await DeleteCompanyApi(CompanyID, CompanyToDelete.CustomerId); });
                } catch (Exception ex) {
                    throw ex;
                }

                Message = "Company has been deleted";
            } else {

                Message = "No record found to delete.";
            }
            return Message;
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<List<CompanyObject>> GetCompanyObject(int CompanyID, string ObjectName) {


        try {
            var ObjectInfo = await _context.Set<CCObj>()
                .Join(_context.Set<ObjectMapping>(), (o => o.ObjectId), (om => om.TargetObjectId), (o, om) => new { o, om })
                              .Where(w => (w.om.CompanyId == CompanyID || w.om.CompanyId == null) && w.o.ObjectName == ObjectName)
                              .Select(s => new CompanyObject {
                                  ObjectMappingId = s.om.ObjectMappingId,
                                  ObjectId = s.om.SourceObjectId,
                                  CompanyId = CompanyID,
                                  ObjectName = _context.Set<CCObj>()
                                                .Where(OBJD => OBJD.ObjectId == s.om.SourceObjectId)
                                                .Select(OBJD => OBJD.ObjectName).FirstOrDefault(),
                                  ObjectTableName = _context.Set<CCObj>()
                                                     .Where(OBJD => OBJD.ObjectId == s.om.SourceObjectId)
                                                     .Select(OBJD => OBJD.ObjectTableName).FirstOrDefault(),
                              }).ToListAsync();

            if (ObjectInfo != null) {
                return ObjectInfo;
            }
            return null;
        } catch (Exception ex) {
            throw ex;
        }
    }
    public async Task<List<GroupUsers>> GetGroupUsers(int GroupId, int ObjectMappingId) {

        try {
            var UserList = await _context.Set<ObjectRelation>().Include(u => u.User)
                            .Where(OR => OR.ObjectMappingId == ObjectMappingId && OR.SourceObjectPrimaryId == GroupId)
                            .Select(U => new GroupUsers {
                                UserId = U.User.UserId,
                                UserFullName = new UserFullName { Firstname = U.User.FirstName, Lastname = U.User.LastName },
                                UserPhoto = U.User.UserPhoto,
                                UserRole = U.User.UserRole,
                                Status = U.User.Status
                            }).ToListAsync();

            if (UserList != null) {
                return UserList;
            }
            return null;
        } catch (Exception ex) {
            throw ex; ;
        }
    }
    #region SCIM Methods
    public async Task<CompanyScimProfile> GetScimProfile(int outUserCompanyId) {
        try {
            var pCompanyID = new SqlParameter("@CompanyID", outUserCompanyId);
            var result = await _context.Set<CompanyScimProfile>().FromSqlRaw("exec Pro_Get_Scim_Profile @CompanyID", pCompanyID).FirstOrDefaultAsync();
            if (result != null) {
                return result;
            }
            return new CompanyScimProfile();
        } catch (Exception ex) {
            throw ex;
        }

    }

    public async Task<CompanyScimProfile> SaveScimProfileAsync(CompanyScim IP, int CurrentUserId, int CompanyId, string TimezoneId)
    {
        try
        {
            DateTimeOffset dtNow = await _DBC.GetDateTimeOffset(DateTime.Now, TimezoneId);

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@CompanyId", CompanyId));
            parameters.Add(new SqlParameter("@ScimToken", IP.ScimToken));
            parameters.Add(new SqlParameter("@TokenExpiry", IP.TokenExpiry));
            parameters.Add(new SqlParameter("@DefaultMenuAccess", IP.DefaultMenuAccess));
            parameters.Add(new SqlParameter("@DefaultGroup", IP.DefaultGroup));
            parameters.Add(new SqlParameter("@DefaultLocation", IP.DefaultLocation));
            parameters.Add(new SqlParameter("@DefaultDepartment", IP.DefaultDepartment));
            parameters.Add(new SqlParameter("@DefaultMobile", IP.DefaultMobile));
            parameters.Add(new SqlParameter("@TokenGenerated", IP.TokenGenerated));
            parameters.Add(new SqlParameter("@TokenGeneratedOn", dtNow));
            parameters.Add(new SqlParameter("@NotificationEmails", IP.NotificationEmails));
            parameters.Add(new SqlParameter("@SendInvitation", IP.SendInvitation));
            parameters.Add(new SqlParameter("@PingMethods", IP.PingMethods));
            parameters.Add(new SqlParameter("@IncidentMethods", IP.IncidentMethods));
            parameters.Add(new SqlParameter("@UpdatedBy", CurrentUserId));
            parameters.Add(new SqlParameter("@UpdatedOn", dtNow));

            var result = await _context.Set<CompanyScimProfile>().FromSqlRaw(
                "exec Pro_Save_Scim_Profile @CompanyID,@ScimToken,@TokenExpiry,@DefaultMenuAccess,@DefaultGroup,@DefaultLocation,@DefaultDepartment,@DefaultMobile," +
                "@TokenGenerated,@TokenGeneratedOn,@NotificationEmails,@SendInvitation,@PingMethods,@IncidentMethods,@UpdatedBy,@UpdatedOn",
                parameters.ToArray()).FirstOrDefaultAsync();
            if (result != null) {
                return result;
            }
            return new CompanyScimProfile();
        } catch (Exception ex) {
            throw ex;
        }

    }
    #endregion SCIM Methods


}