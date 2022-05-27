using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrisesControl.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CrisesControlContext _context;
    private readonly string timeZoneId = "GMT Standard Time";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int userID;
    private int companyID;
    private readonly ILogger<UserRepository> _logger;
    const string action = "ADD";


    public UserRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserRepository> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        userID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
        companyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        _logger=logger;
    }

    public async Task<int> CreateUser(User user, CancellationToken cancellationToken)
    {
        await _context.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return user.UserId;
    }

    public bool EmailExists(string email)
    {
        return _context.Set<User>().Any(x => x.PrimaryEmail == email);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task<int> UpdateUser(User user, CancellationToken cancellationToken)
    {
        var result = _context.Set<User>().Where(t => t.UserId == user.UserId).FirstOrDefault();

        if (result == null)
        {
            return default;
        }
        else
        {
            result.ActiveOffDuty = user.ActiveOffDuty;
            result.CompanyId = user.CompanyId;
            result.CreatedBy = user.CreatedBy;
            result.CreatedOn = user.CreatedOn;
            result.DepartmentId = user.DepartmentId;
            result.ExpirePassword = user.ExpirePassword;
            result.FirstLogin = user.FirstLogin;
            result.FirstName = user.FirstName;
            result.Isdcode = user.Isdcode;
            result.Landline = user.Landline;
            result.LastLocationUpdate = user.LastLocationUpdate;
            result.LastName = user.LastName;
            result.Lat = user.Lat;
            result.Lng = user.Lng;
            result.Llisdcode = user.Llisdcode;
            result.MobileNo = user.MobileNo;
            result.Otpcode = user.Otpcode;
            result.Otpexpiry = user.Otpexpiry;
            result.Password = user.Password;
            result.PasswordChangeDate  = user.PasswordChangeDate;
            result.PrimaryEmail = user.PrimaryEmail;
            result.RegisteredUser = user.RegisteredUser;
            result.SecondaryEmail = user.SecondaryEmail;
            result.Smstrigger = user.Smstrigger;
            result.Status = user.Status;
            result.TimezoneId = user.TimezoneId;
            result.TrackingEndTime = user.TrackingEndTime;
            result.TrackingStartTime = user.TrackingStartTime;
            result.UniqueGuiId = user.UniqueGuiId;
            result.UpdatedBy = user.UpdatedBy;
            result.UpdatedOn = user.UpdatedOn;
            result.UserLanguage = user.UserLanguage;
            result.UserPhoto = user.UserPhoto;
            result.UserHash = user.UserHash;
            result.UserId = user.UserId;
            result.UserRole = user.UserRole;
            await _context.SaveChangesAsync(cancellationToken);
            return result.UserId;
        }
    }

    public int AddPwdChangeHistory(int userId, string newPassword, string timeZoneId)
    {

        var ph = new PasswordChangeHistory
        {
            UserId = userId,
            LastPassword = newPassword,
            ChangedDateTime = DateTime.Now.GetDateTimeOffset(timeZoneId)
        };

        _context.Add(ph);
        _context.SaveChanges();
        return ph.Id;

    }

    public async Task CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId)
    {
        var searchString = firstName + " " + lastName + "|" + primaryEmail + "|" + isdCode + mobileNo;

        var comp = await _context.Set<Company>().Include(std=>std.StdTimeZone).Include(pk=>pk.PackagePlan).FirstOrDefaultAsync(x => x.CompanyId == companyId);
        if (comp != null)
        {
            var memberUser = _context.Set<MemberUser>().FromSqlRaw(" exec Pro_Create_User_Search {0}, {1}, {2}",
                userId, searchString, comp.UniqueKey!).FirstOrDefault();
        }
    }

    public async Task<User> DeleteUser(User user, CancellationToken token)
    {
        var userToRemove = new User { UserId = user.UserId, CompanyId =  user.CompanyId};
        _context.Remove(userToRemove);
        await _context.SaveChangesAsync(token);
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers(GetAllUserRequest request)
    {
        var propertyInfo = typeof(GetAllUserRequest).GetProperty(request.OrderBy);
        var val = _context.Set<User>().FromSqlRaw($"Pro_Get_User_SelectAll @CompanyId = {companyID},@UserID = {userID},@RecordStart={request.RecordStart},@RecordLength={request.RecordLength},@SearchString={request.SearchString}," +
            $"@OrderBy = {request.OrderBy},@SkipDeleted= {request.SkipDeleted},@ActiveOnly = {request.ActiveOnly},@SkipInActive={request.SkipInActive},@KeyHolderOnly={request.KeyHolderOnly},@Filters={request.Filters},@UniqueKey = {request.CompanyKey}").OrderByDescending(o => propertyInfo.GetValue(o, null)).ToListAsync();
        return await _context.Set<User>().Where(t => t.CompanyId == companyID).ToListAsync();
    }

    public async Task<User> GetUser(int companyId, int userId)
    {
        return await _context.Set<User>().Where(t => t.CompanyId == companyId && t.UserId == userId).FirstOrDefaultAsync();
    }

    public bool CheckDuplicate(User user)
    {
        return _context.Set<User>().Where(t => t.PrimaryEmail.Equals(user.PrimaryEmail)).Any();
    }

    private string LookupWithKey(string Key, string Default = "")
    {
        try
        {
            var LKP = (from L in _context.Set<SysParameter>()
                       where L.Name == Key
                       select L).FirstOrDefault();
            if (LKP != null)
            {
                Default = LKP.Value;
            }
            return Default;
        }
        catch (Exception ex)
        {
            return Default;
        }
    }

    private string[] CCRoles(bool AddKeyHolder = false, bool AddUser = false)
    {
        List<string> rolelist = new List<string> { "ADMIN", "SUPERADMIN" };
        if (AddKeyHolder)
            rolelist.Add("KEYHOLDER");

        if (AddUser)
            rolelist.Add("USER");

        return rolelist.ToArray();
    }

    private string GetAppVersion(string DeviceType)
    {
        string AppVersion;
        string AppTypeKey = string.Empty;
        if (DeviceType.Contains("Android"))
        {
            AppTypeKey = "ANDROID_VERSION";
        }
        else if (DeviceType.Contains("iPad") || DeviceType.Contains("iPhone") || DeviceType.Contains("iPod"))
        {
            AppTypeKey = "APPLE_VERSION";
        }
        else if (DeviceType.Contains("Windows"))
        {
            AppTypeKey = "WINDOWS_VERSION";
        }
        else if (DeviceType.Contains("Blackberry"))
        {
            AppTypeKey = "BB_VERSION";
        }
        AppVersion = LookupWithKey(AppTypeKey);
        return AppVersion;
    }

    private DateTimeOffset GetDateTimeOffset(DateTime CrTime, string TimeZoneId = "GMT Standard Time")
    {
        try
        {
            if (CrTime.Year <= 2000)
                return CrTime;

            if (CrTime.Year > 3000)
            {
                CrTime = DateTime.MaxValue.AddHours(-48);
            }

            TimeZoneInfo cet = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var offset = cet.GetUtcOffset(CrTime);

            DateTimeOffset newvals = new DateTimeOffset(new DateTime(CrTime.Year, CrTime.Month, CrTime.Day, CrTime.Hour, CrTime.Minute, CrTime.Second, CrTime.Millisecond));

            DateTimeOffset convertedtime = newvals.ToOffset(offset);

            return convertedtime;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private  DateTime GetLocalTime(string TimeZoneId, DateTime? ParamTime = null)
    {
        try
        {
            if (string.IsNullOrEmpty(TimeZoneId))
                TimeZoneId = "GMT Standard Time";

            DateTime retDate = DateTime.Now.ToUniversalTime();

            DateTime dateTimeToConvert = new DateTime(retDate.Ticks, DateTimeKind.Unspecified);

            DateTime timeUtc = DateTime.UtcNow;

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            retDate = TimeZoneInfo.ConvertTimeFromUtc(dateTimeToConvert, cstZone);

            return retDate;
        }
        catch (Exception ex) { throw ex; }
        return DateTime.Now;
    }

    private string Left(string str, int lngth, int stpoint = 0)
    {
        return str.Substring(stpoint, Math.Min(str.Length, lngth));
    }

    public async Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "")
    {
        try
        {
            Key = Key.ToUpper();

            if (CompanyId > 0)
            {
                var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                else
                {

                    var LPR = await _context.Set<LibCompanyParameter>().Where(CP => CP.Name == Key).FirstOrDefaultAsync();
                    if (LPR != null)
                    {
                        Default = LPR.Value;
                    }
                    else
                    {
                        Default =  LookupWithKey(Key, Default);
                    }
                }
            }

            if (!string.IsNullOrEmpty(CustomerId) && !string.IsNullOrEmpty(Key))
            {

                var cmp = await _context.Set<Company>().Where(w => w.CustomerId == CustomerId).FirstOrDefaultAsync();
                if (cmp != null)
                {
                    var LKP = await _context.Set<CompanyParameter>().Where(CP => CP.Name == Key && CP.CompanyId == CompanyId).FirstOrDefaultAsync();
                    if (LKP != null)
                    {
                        Default = LKP.Value;
                    }
                }
                else
                {
                    Default = "NOT_EXIST";
                }
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

    public async Task<LoginInfoReturnModel> GetLoggedInUserInfo(LoginInfo request, CancellationToken cancellationToken)
    {
        try
        {
            var CompanyInfo = await (from C in _context.Set<Company>()
                               join TZ in _context.Set<StdTimeZone>() on C.TimeZone equals TZ.TimeZoneId
                               where C.CompanyId == companyID
                                     select new { C, TZ }).FirstOrDefaultAsync();

            string strCompanyName = "";

            if (CompanyInfo != null) {
                strCompanyName = CompanyInfo.C.CompanyName;
                UserLoginLog TblUserLog = new UserLoginLog();
                TblUserLog.CompanyId = companyID;
                TblUserLog.UserId = userID;
                TblUserLog.DeviceType = request.DeviceType;
                TblUserLog.Ipaddress = request.IPAddress;
                TblUserLog.LoggedInTime = GetDateTimeOffset(DateTime.Now, timeZoneId);
                await _context.AddAsync(TblUserLog);
                await _context.SaveChangesAsync(cancellationToken);


                if (!string.IsNullOrEmpty(request.Language)) {
                    var user = await (from Usersval in _context.Set<User>()
                                where Usersval.CompanyId == companyID && Usersval.UserId == userID
                                      select Usersval).FirstOrDefaultAsync();
                    if (user != null) {
                        user.UserLanguage = request.Language;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                var RegUserInfo = await (from U in _context.Set<User>()
                                   join TZ in _context.Set<StdTimeZone>() on U.TimezoneId equals TZ.TimeZoneId into ps
                                   from p in ps.DefaultIfEmpty()
                                   where U.CompanyId == companyID && U.UserId == userID
                                         select new LoginInfoReturnModel {
                                       CompanyId = U.CompanyId,
                                       CompanyName = strCompanyName,
                                       CompanyLogo = CompanyInfo.C.CompanyLogoPath,
                                       CompanyProfile = CompanyInfo.C.CompanyProfile,
                                       AnniversaryDate = CompanyInfo.C.AnniversaryDate,
                                       UserId = U.UserId,
                                       CustomerId = CompanyInfo.C.CustomerId,
                                       First_Name = U.FirstName,
                                       Last_Name = U.LastName,
                                       UserMobileISD = U.Isdcode,
                                       MobileNo = U.MobileNo,
                                       Primary_Email = U.PrimaryEmail,
                                       UserPassword = U.Password,
                                       UserPhoto = U.UserPhoto,
                                       UniqueGuiId = U.UniqueGuiId,
                                       RegisterUser = U.RegisteredUser,
                                       UserRole = U.UserRole,
                                       UserLanguage = U.UserLanguage,
                                       Status = U.Status,
                                       FirstLogin = U.FirstLogin,
                                       CompanyPlanId = (int)CompanyInfo.C.PackagePlanId,
                                       CompanyStatus = CompanyInfo.C.Status,
                                       UniqueKey = CompanyInfo.C.UniqueKey,
                                       PortalTimeZone = p != null ? p.PortalTimeZone : CompanyInfo.TZ.PortalTimeZone,
                                       ActiveOffDuty = U.ActiveOffDuty,
                                       TimeZoneId = CompanyInfo.TZ.TimeZoneId,
                                       SecItems = (from PF in _context.Set<CompanyPackageFeature>()
                                                   join SO in _context.Set<SecurityObject>() on PF.SecurityObjectId equals SO.SecurityObjectId
                                                   join SOT in _context.Set<SecurityObjectType>() on SO.TypeId equals SOT.SecurityObjectTypeId
                                                   join ASG in
                                                       (from USG in _context.Set<UserSecurityGroup>()
                                                        join SG in _context.Set<SecurityGroup>() on USG.SecurityGroupId equals SG.SecurityGroupId
                                                        join GSO in _context.Set<GroupSecuityObject>() on USG.SecurityGroupId equals GSO.SecurityGroupId
                                                        join SO1 in _context.Set<SecurityObject>() on GSO.SecurityObjectId equals SO1.SecurityObjectId
                                                        where USG.UserId == U.UserId && SO1.Status == 1 && SG.CompanyId == companyID
                                                        select new { USG.SecurityGroupId, SO1.SecurityObjectId })
                                                   on SO.SecurityObjectId equals ASG.SecurityObjectId
                                                   into ASGS
                                                   from ASG in ASGS.DefaultIfEmpty()
                                                   where SO.Status == 1 && PF.Status == 1 && PF.CompanyId == U.CompanyId
                                                   //&& SO.Target.Contains("Client")
                                                   select new SecItemModel {
                                                       Code = SOT.Code,
                                                       SecurityKey = SO.SecurityKey,
                                                       Name = SO.Name,
                                                       UpdatedOn = SO.UpdatedOn,
                                                       Target = SO.Target,
                                                       ShowOnTrial = (bool)SO.ShowOnTrial,
                                                       HasAccess = ASG.SecurityGroupId == null ||
                                                       (U.UserRole == "USER" && SO.RequireKeyHolder == true) ||
                                                       ((U.UserRole == "USER" || U.UserRole == "KEYHOLDER") && SO.RequireAdmin == true)
                                                       ? "false" : "true"
                                                   }).ToList(),
                                       ErrorId = 0,
                                       Message = "OK"
                                   }).FirstOrDefaultAsync();
                return RegUserInfo;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }   


    public async Task<List<MemberUser>> MembershipList(int ObjMapID, MemberShipType memberShipType, int TargetID, int? Start, int? Length, string? Search, string orderBy, string orderDir, bool ActiveOnly, string? CompanyKey)
    {
        try
        {

            var SearchString = (Search != null) ? Search : string.Empty;           


            var pCompanyId = new SqlParameter("@CompanyID", companyID);
            var pUserID = new SqlParameter("@UserID", userID);
            var pObjMapID = new SqlParameter("@ObjMapID", ObjMapID);
            var pTargetID = new SqlParameter("@TargetID", TargetID);
            var pRecordStart = new SqlParameter("@RecordStart", Start);
            var pRecordLength = new SqlParameter("@RecordLength", Length);
            var pSearchString = new SqlParameter("@SearchString", SearchString);
            var pOrderBy = new SqlParameter("@OrderBy", orderBy);
            var pOrderDir = new SqlParameter("@OrderDir", orderDir);
            var pActiveOnly = new SqlParameter("@ActiveOnly", ActiveOnly);
            var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

            var MainUserlist = new List<MemberUser>();
            var propertyInfo = typeof(MemberUser).GetProperty(orderBy);
            if (memberShipType.ToMemString().ToUpper() == MemberShipType.NON_MEMBER.ToMemString().ToUpper())
            {
                if (orderDir == "desc" && propertyInfo != null)
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_Members,  @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                        .ToListAsync();


                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                }
                else if (orderDir == "asc" && propertyInfo != null)
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_Members, @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                        .ToListAsync();



                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }
                else
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_Members @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey).ToListAsync();



                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    }).ToList();
                }

                return MainUserlist;
            }

            else
            {
                if (orderDir == "desc" && propertyInfo != null)
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_NonMembers,  @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                        .ToListAsync();

                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                        .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
                }
                else if (orderDir == "asc" && propertyInfo != null)
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_NonMembers, @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                        .ToListAsync();


                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
                }
                else
                {
                    MainUserlist = await _context.Set<MemberUser>().FromSqlRaw("exec Pro_Get_Group_NonMembers @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                           pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                        .ToListAsync();


                    MainUserlist.Select(c =>
                    {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    }).ToList();
                }

                return MainUserlist;
            }

           
        }
        catch (Exception ex)
        {
            return null;
        }
        
    }

    private string GetCompanyName(int companyId)
    {
        string companyName =  _context.Set<Company>().Where(c=>c.CompanyId == companyId).Select(c=>c.CompanyName).FirstOrDefault();
        return companyName;
    }

    public async Task<User> ReactivateUser(int queriedUserId, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await _context.Set<User>().Where(t => t.UserId == queriedUserId).FirstOrDefaultAsync();

            if (userRecord != null)
            {
                userRecord.Status = 1;
                await _context.SaveChangesAsync(cancellationToken);
                var ActivatedUser = _context.Set<User>().Where(u=> u.UserId == queriedUserId).Select(u=> new
                                     {
                                         UserId = u.UserId,
                                         UserName = new UserFullName { Firstname = u.FirstName, Lastname = u.LastName },
                                         UserEmail = u.PrimaryEmail,
                                         CompanyName = GetCompanyName(u.CompanyId)
                                     }).FirstOrDefault();
                if (ActivatedUser != null)
                {
                    return userRecord;
                }
                else
                {
                    return null;
                }
            }
           
           return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

 
    public async Task<ValidateEmailReponseModel> ValidateLoginEmail(string userName)
    {
        try
        {
            var user = (from U in _context.Set<User>()
                        join C in _context.Set<Company>() on U.CompanyId equals C.CompanyId
                        where U.PrimaryEmail == userName && U.Status == 1
                        select new { U.UserId, U.Password, C.CompanyId }).FirstOrDefault();
            if (user != null)
            {
                string sso_type = await GetCompanyParameter("SSO_PROVIDER", user.CompanyId);
                string sso_enabled = await GetCompanyParameter("SINGLE_SIGNON_ENABLED", user.CompanyId);
                string sso_issuer = await GetCompanyParameter("AAD_SSO_TENANT_ID", user.CompanyId);
                string sso_client_secret = await GetCompanyParameter("SSO_CLIENT_SECRET", user.CompanyId); 
                return new ValidateEmailReponseModel 
                { 
                    SSOType = sso_type, 
                    SSOEnabled = sso_enabled, 
                    SSOIssuer = sso_issuer, 
                    SSOSecret = sso_client_secret 
                };
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<int> UpdateProfile(User user)
    {
       _context.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"User Profile has been updated {user.UserId}");
        return user.UserId;
    }
    public async void UserCommsMethods(int UserId, string MethodType, int[] MethodId, int CurrentUserID, int CompanyID, string TimeZoneId)
    {
        try
        {
            if (MethodId.Count() > 0)
            {
                var DelCommList = await _context.Set<UserComm>().Where(UC => UC.UserId == UserId && UC.MessageType == MethodType).ToListAsync();

                List<int> ExtComList = new List<int>();
                foreach (int NewMethodId in MethodId)
                {
                    var ISEXsit =  DelCommList.FirstOrDefault(S => S.UserId == UserId && S.MethodId == NewMethodId && S.MessageType == MethodType);
                    if (ISEXsit == null)
                    {
                        CreateUserComms(UserId, CompanyID, NewMethodId, CurrentUserID, TimeZoneId, MethodType);
                    }
                    else
                    {
                        ExtComList.Add(ISEXsit.UserCommsId);
                    }
                }

                foreach (var item in DelCommList)
                {
                    bool ISDEL = ExtComList.Any(s => s == item.UserCommsId);
                    if (!ISDEL)
                    {
                        item.Status = 0;
                    }
                    else
                    {
                        item.Status = 1;
                    }
                }
              await  _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source},{5}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source, CompanyID) ;
        }
      }
    public async void UserCommsPriority(int UserID, List<CommsMethodPriority> CommsMethod, int CurrentUserID, int CompanyID,CancellationToken token)
    {
        try
        {

            string PriorityChangeAllowed = await GetCompanyParameter(KeyType.ALLOWCHANGEPRIORITYUSER.ToDbKeyString(), CompanyID);

            if (CommsMethod.Count() > 0 && PriorityChangeAllowed == "true")
            {

                var CommsList = await _context.Set<UserComm>().Where(UC => UC.UserId == UserID).ToListAsync();

                foreach (var Comms in CommsList)
                {
                    var priority = CommsMethod.Where(w => w.MessageType == Comms.MessageType && w.MethodId == Comms.MethodId).FirstOrDefault();
                    if (priority != null)
                    {
                        Comms.Priority = priority.Priority;
                    }
                }
               await _context.SaveChangesAsync(token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source},{5}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source, CompanyID);
        }
    }
    public async void CreateUserComms(int UserId, int CompanyId, int MethodId, int CreatedUpdatedBy, string TimeZoneId, string CommType)
    {
        try
        {
            var IsCommExist = await _context.Set<UserComm>().Where(UCMM => UCMM.UserId == UserId && UCMM.CompanyId == CompanyId
                             && UCMM.MethodId == MethodId
                            && UCMM.MessageType == CommType).FirstOrDefaultAsync();
            if (IsCommExist == null)
            {
                UserComm NewUserComms = new UserComm()
                {
                    UserId = UserId,
                    CompanyId = CompanyId,
                    MethodId = MethodId,
                    MessageType = CommType,
                    Status = 1,
                    Priority = 1,
                    CreatedBy = CreatedUpdatedBy,
                    UpdatedBy = CreatedUpdatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = GetDateTimeOffset(DateTime.Now, TimeZoneId)
                };
                await _context.AddAsync(NewUserComms);
                await _context.SaveChangesAsync();
            }
            else
            {
                IsCommExist.Status = 1;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex) {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
    }
    public async void CreateSMSTriggerRight(int CompanyId, int UserId, string UserRole, bool SMSTrigger, string ISDCode, string MobileNo, bool Self = false)
    {
        try
        {
            var roles = Roles.CcRoles(true); ;
           var checkusr= await _context.Set<SmsTriggerUser>().FirstOrDefaultAsync(STU=> STU.CompanyId == CompanyId && STU.UserId == UserId);
            if (checkusr != null)
            {

                if (roles.Contains(UserRole.ToUpper()))
                {
                    checkusr.PhoneNumber = ISDCode + MobileNo;
                }
                else
                {
                    SMSTrigger = false;
                }

                if (!SMSTrigger)
                    _context.Remove(checkusr);

                await _context.SaveChangesAsync();
            }
            else
            {
                if (roles.Contains(UserRole.ToUpper()) && SMSTrigger == true)
                {
                    SmsTriggerUser STU = new SmsTriggerUser();
                    STU.CompanyId = CompanyId;
                    STU.UserId = UserId;
                    STU.PhoneNumber = ISDCode + MobileNo;
                    STU.Status = 1;
                   _context.AddAsync(STU);
                    _context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<User> GetRegisteredUserInfo(int CompanyId, int userId)
    {
        try { 
        var RegUserInfo = await _context.Set<User>().Include(uc => uc.UserComm).Include(usg=>usg.UserSecurityGroup).Where(Usersval => Usersval.CompanyId == CompanyId && Usersval.UserId == userId).FirstOrDefaultAsync();

        if (RegUserInfo != null)
        {
            return RegUserInfo;
        }
        return null;
        }
        catch(Exception ex)
        {
         return null;
        }
    }

    public async Task<bool> UpdateUserMsgGroups(List<UserGroup> UserGroups)
    {
        try
        {
            StringBuilder usb = new StringBuilder();

            foreach (var UsrGrp in UserGroups)
            {

                var usg = await _context.Set<ObjectRelation>().Where(grp => grp.ObjectRelationId == UsrGrp.UniqueId).FirstOrDefaultAsync();
                if (usg != null)
                    usg.ReceiveOnly = UsrGrp.ReceiveOnly;
                 usb.AppendLine(_context.Update(usg).ToString());
                
            }
           await _context.SaveChangesAsync();
            
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(" Error occured while seeding the database {1}", ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateGroupMember(int TargetID, int UserID, int ObjMapID, string Action)
    {
        try
        {
            int CurrentUserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
            int CompanyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
            if (Action.ToUpper() == action.ToUpper() && ObjMapID > 0)
            {
                await CreateNewObjectRelation(TargetID, UserID, ObjMapID, CurrentUserId, timeZoneId);
            }
            else if (ObjMapID > 0)
            {
                var DelOBjs = await _context.Set<ObjectRelation>().Where(OJR => OJR.ObjectMappingId == ObjMapID
                                                                  && OJR.SourceObjectPrimaryId == TargetID && OJR.TargetObjectPrimaryId == UserID).FirstOrDefaultAsync();

                if (DelOBjs != null)
                {
                    _context.Set<ObjectRelation>().Remove(DelOBjs);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                await UpdateUserDepartment(UserID, TargetID, Action, CurrentUserId, CompanyId, timeZoneId);
            }

            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }
    public async Task UpdateUserDepartment(int UserID, int DepartmentID, string Action, int CurrentUserId, int CompanyId, string TimeZoneId)
    {
        try
        {

            var user = await _context.Set<User>().Where(U => U.UserId == UserID && U.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (user != null)
            {
                if (Action.ToUpper() ==action.ToUpper())
                {
                    user.DepartmentId = DepartmentID;
                }
                else
                {
                    user.DepartmentId = 0;
                }
                user.UpdatedOn = GetDateTimeOffset(DateTime.Now, TimeZoneId);
                user.UpdatedBy = CurrentUserId;
                _context.Update(user);
               await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured while seeding the database {0}, {1}", ex.Message, ex.InnerException);
        }
    }
    public async Task CreateNewObjectRelation(int SourceObjectID, int TargetObjectID, int ObjMapId, int CreatedUpdatedBy, string TimeZoneId)
    {
        try
        {
           
            bool IsALLOBJrelationExist = await _context.Set<ObjectRelation>().Where(OBR => OBR.TargetObjectPrimaryId == TargetObjectID
                                          && OBR.ObjectMappingId == ObjMapId
                                          && OBR.SourceObjectPrimaryId == SourceObjectID).AnyAsync();
            if (!IsALLOBJrelationExist)
            {
                ObjectRelation tblDepObjRel = new ObjectRelation()
                {
                    TargetObjectPrimaryId = TargetObjectID,
                    ObjectMappingId = ObjMapId,
                    SourceObjectPrimaryId = SourceObjectID,
                    CreatedBy = CreatedUpdatedBy,
                    UpdatedBy = CreatedUpdatedBy,
                    CreatedOn = System.DateTime.Now,
                    UpdatedOn = GetLocalTime(TimeZoneId,DateTime.UtcNow),
                    ReceiveOnly = false
                };
               await _context.AddAsync(tblDepObjRel);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured while seeding the database {0}, {1}", ex.Message, ex.InnerException);
        }
    }

}