using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrisesControl.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CrisesControlContext _context;
    private readonly string timeZoneId = "GMT Standard Time";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SendEmail _SDE;
    private readonly DBCommon _DBC;
    private readonly LocationRepository _locationRepository;
    private readonly GroupRepository _groupRepository;

    private int userId;
    private int companyId;
    private readonly ILogger<UserRepository> _logger;
    const string action = "ADD";


    public UserRepository(
        CrisesControlContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserRepository> logger,
        SendEmail SDE,
        DBCommon DBC,
        LocationRepository locationRepository,
        GroupRepository groupRepository
        )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
        companyId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
        _logger = logger;
        _SDE = SDE;
        _DBC = DBC;
        _locationRepository = locationRepository;
        _groupRepository = groupRepository;
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
            result.PasswordChangeDate = user.PasswordChangeDate;
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

        var comp = await _context.Set<Company>().Include(std => std.StdTimeZone).Include(pk => pk.PackagePlan).FirstOrDefaultAsync(x => x.CompanyId == companyId);
        if (comp != null)
        {
            var memberUser = _context.Set<MemberUser>().FromSqlRaw(" exec Pro_Create_User_Search {0}, {1}, {2}",
                userId, searchString, comp.UniqueKey!).FirstOrDefault();
        }
    }

    public async Task<User> DeleteUser(User user, CancellationToken cancellationToken)
    {
        var userToBeDeleted = await _context.Set<User>().Where(t => t.CompanyId == user.UserId).FirstOrDefaultAsync();

        if (userToBeDeleted != null)
        {
            if (userToBeDeleted.RegisteredUser != true)
            {
                userToBeDeleted.Status = 3;
                //tblUser.TOKEN = "";
                userToBeDeleted.PrimaryEmail = "DEL-" + user.PrimaryEmail;
                userToBeDeleted.UserHash = _DBC.PWDencrypt(user.PrimaryEmail);
                userToBeDeleted.UpdatedBy = userId;
                userToBeDeleted.UpdatedOn = _DBC.GetLocalTime(timeZoneId, System.DateTime.Now);
                await _context.SaveChangesAsync(cancellationToken);

                await CheckUserAssociation(user.UserId, user.CompanyId, cancellationToken);

                //Remove all user devices;
                RemoveUserDevice(user.UserId);
                DeleteUsercoms(user.UserId, 0, true);

                //_billing.AddUserRoleChange(CompanyId, UserId, tblUser.UserRole, TimeZoneId);

            }
            return user;
        }
        throw new UserNotFoundException(companyId, userId);
    }

    private void DeleteUsercoms(int userId, int methodId, bool deleteAll = false)
    {
        try
        {
            if (!deleteAll)
            {
                var delRec = _context.Set<UserComm>().Where(userComm => userComm.UserId == userId && userComm.MethodId == methodId).FirstOrDefault();
                if (delRec != null)
                {
                    _context.Set<UserComm>().Remove(delRec);
                    _context.SaveChanges();
                }
            }
            else
            {
                var delRec = _context.Set<UserComm>().Where(userComm => userComm.UserId == userId).ToList();
                if (delRec != null)
                {
                    _context.Set<UserComm>().RemoveRange(delRec);
                    _context.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            //ToDo: throw exception
            throw ex;
        }
    }

    private void RemoveUserDevice(int userId, bool tokenReset = false)
    {
        try
        {
            var devices = _context.Set<UserDevice>().Where(userDevice => userDevice.UserId == userId).ToList();
            if (!tokenReset)
            {
                _context.Set<UserDevice>().RemoveRange(devices);
            }
            else
            {
                devices.ForEach(s => s.DeviceToken = "");
            }
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
            //ToDo: throw exception
        }
    }

    private async Task<bool> CheckUserAssociation(int qUserId, int qCompanyId, CancellationToken cancellationToken)
    {
        try
        {
            bool sendemail = false;

            var result = await _context.Set<ModuleLinks>().FromSqlRaw("EXEC Pro_Get_User_Association @UserID, @CompanyID", qUserId, qCompanyId).ToListAsync();
            if (result.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<table width=\"100%\" class=\"user_list\" style=\"border:1px solid #000000;border-collapse: collapse\"><tr width=\"25%\"><th style=\"text-align:left;\">Module</th><th width=\"75%\" style=\"text-align:left;\">Item Name</th></tr>");
                sendemail = true;
                foreach (var item in result)
                {
                    sb.AppendLine("<tr><td>" + item.Module + "</td><td>" + item.ModuleItem + "</td></tr>");
                }
                sb.AppendLine("</table>");
                _SDE.SendUserAssociationsToAdmin(sb.ToString(), qUserId, qCompanyId);
            }

            return sendemail;
        }
        catch (Exception ex)
        {
            throw ex;
            ///ToDo: throw exception
        }
    }

    public async Task<IEnumerable<User>> GetAllUsers(GetAllUserRequestList request)
    {
        var propertyInfo = typeof(GetAllUserRequestList).GetProperty(request.OrderBy);
        var val = _context.Set<User>().FromSqlRaw($"Pro_Get_User_SelectAll @CompanyId = {companyId},@UserID = {userId},@RecordStart={request.RecordStart},@RecordLength={request.RecordLength},@SearchString={request.SearchString}," +
            $"@OrderBy = {request.OrderBy},@SkipDeleted= {request.SkipDeleted},@ActiveOnly = {request.ActiveOnly},@SkipInActive={request.SkipInActive},@KeyHolderOnly={request.KeyHolderOnly},@Filters={request.Filters},@UniqueKey = {request.CompanyKey}").OrderByDescending(o => propertyInfo.GetValue(o, null)).ToListAsync();
        return await _context.Set<User>().Where(t => t.CompanyId == companyId).ToListAsync();
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
    private DateTime GetLocalTime(string TimeZoneId, DateTime? ParamTime = null)
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
                        Default = LookupWithKey(Key, Default);
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
                                     where C.CompanyId == companyId
                                     select new { C, TZ }).FirstOrDefaultAsync();

            string strCompanyName = "";

            if (CompanyInfo != null)
            {
                strCompanyName = CompanyInfo.C.CompanyName;
                UserLoginLog TblUserLog = new UserLoginLog();
                TblUserLog.CompanyId = companyId;
                TblUserLog.UserId = userId;
                TblUserLog.DeviceType = request.DeviceType;
                TblUserLog.Ipaddress = request.IPAddress;
                TblUserLog.LoggedInTime = GetDateTimeOffset(DateTime.Now, timeZoneId);
                await _context.AddAsync(TblUserLog);
                await _context.SaveChangesAsync(cancellationToken);


                if (!string.IsNullOrEmpty(request.Language))
                {
                    var user = await (from Usersval in _context.Set<User>()
                                      where Usersval.CompanyId == companyId && Usersval.UserId == userId
                                      select Usersval).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        user.UserLanguage = request.Language;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                var RegUserInfo = await (from U in _context.Set<User>()
                                         join TZ in _context.Set<StdTimeZone>() on U.TimezoneId equals TZ.TimeZoneId into ps
                                         from p in ps.DefaultIfEmpty()
                                         where U.CompanyId == companyId && U.UserId == userId
                                         select new LoginInfoReturnModel
                                         {
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
                                                              where USG.UserId == U.UserId && SO1.Status == 1 && SG.CompanyId == companyId
                                                              select new { USG.SecurityGroupId, SO1.SecurityObjectId })
                                                         on SO.SecurityObjectId equals ASG.SecurityObjectId
                                                         into ASGS
                                                         from ASG in ASGS.DefaultIfEmpty()
                                                         where SO.Status == 1 && PF.Status == 1 && PF.CompanyId == U.CompanyId
                                                         //&& SO.Target.Contains("Client")
                                                         select new SecItemModel
                                                         {
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


            var pCompanyId = new SqlParameter("@CompanyID", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
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
        return null;
    }

    private string GetCompanyName(int companyId)
    {
        string companyName = _context.Set<Company>().Where(c => c.CompanyId == companyId).Select(c => c.CompanyName).FirstOrDefault();
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
                var ActivatedUser = _context.Set<User>().Where(u => u.UserId == queriedUserId).Select(u => new
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
                    var ISEXsit = DelCommList.FirstOrDefault(S => S.UserId == UserId && S.MethodId == NewMethodId && S.MessageType == MethodType);
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
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source},{5}",
                                      ex.Message, ex.StackTrace, ex.InnerException, ex.Source, CompanyID);
        }
    }
    public async void UserCommsPriority(int UserID, List<CommsMethodPriority> CommsMethod, int CurrentUserID, int CompanyID, CancellationToken token)
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
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
        }
    }
    public async void CreateSMSTriggerRight(int CompanyId, int UserId, string UserRole, bool SMSTrigger, string ISDCode, string MobileNo, bool Self = false)
    {
        try
        {
            var roles = Roles.CcRoles(true); ;
            var checkusr = await _context.Set<SmsTriggerUser>().FirstOrDefaultAsync(STU => STU.CompanyId == CompanyId && STU.UserId == UserId);
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

    public async Task<List<GetAllUserDevices>> GetAllUserDeviceList(GetAllUserDeviceRequest request, CancellationToken cancellation)
    {
        try
        {
            var pCompanyId = new SqlParameter("@CompanyId", companyId);
            var pUserID = new SqlParameter("@UserID", userId);
            var pRecordStart = new SqlParameter("@RecordStart", request.RecordStart);
            var pRecordLength = new SqlParameter("@RecordLength", request.RecordLength);
            var pSearchString = new SqlParameter("@SearchString", request.SearchString);
            var pOrderBy = new SqlParameter("@OrderBy", request.OrderBy);
            var pOrderDir = new SqlParameter("@OrderDir", request.OrderDir);
            var pUniqueKey = new SqlParameter("@UniqueKey", request.CompanyKey);

            var UserDeviceList = new List<GetAllUserDevices>();
            var propertyInfo = typeof(GetAllUserDeviceRequest).GetProperty(request.OrderBy);

            if (request.OrderDir == "desc" && propertyInfo != null)
            {
                UserDeviceList = await _context.Set<GetAllUserDevices>().FromSqlRaw("EXEC Pro_Get_User_Devices_SelectAll @CompanyId,@UserID,@RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@UniqueKey",
                    pCompanyId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToListAsync();
                UserDeviceList.Select(c =>
                {
                    c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                }).OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else if (request.OrderDir == "asc" && propertyInfo != null)
            {
                UserDeviceList = await _context.Set<GetAllUserDevices>().FromSqlRaw("EXEC Pro_Get_User_Devices_SelectAll @CompanyId,@UserID,@RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@UniqueKey",
                    pCompanyId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToListAsync();
                UserDeviceList.Select(c =>
                {
                    c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                    return c;
                })
                        .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else
            {
                UserDeviceList = await _context.Set<GetAllUserDevices>().FromSqlRaw("EXEC Pro_Get_User_Devices_SelectAll @CompanyId,@UserID,@RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@UniqueKey",
                    pCompanyId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey)
                    .ToListAsync();
                UserDeviceList.Select(c =>
                    {
                        c.UserName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        return c;
                    }).ToList();
            }

            return UserDeviceList;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
            throw ex;
        }
    }
    public async Task<User> GetRegisteredUserInfo(int CompanyId, int userId)
    {
        try
        {
            var RegUserInfo = await _context.Set<User>().Include(uc => uc.UserComm).Include(usg => usg.UserSecurityGroup).Where(Usersval => Usersval.CompanyId == CompanyId && Usersval.UserId == userId).FirstOrDefaultAsync();

            if (RegUserInfo != null)
            {
                return RegUserInfo;
            }
            return null;
        }
        catch (Exception ex)
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
        catch (Exception ex)
        {
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
                if (Action.ToUpper() == action.ToUpper())
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
                    UpdatedOn = GetLocalTime(TimeZoneId, DateTime.UtcNow),
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

    public async Task<User> DeleteRegisteredUser(int CustomerId, string UniqueGUID, CancellationToken cancellationToken)
    {
        var DeleteUser = _context.Set<User>().Where(u => u.CompanyId == CustomerId && u.UniqueGuiId == UniqueGUID && u.RegisteredUser == false)
            .FirstOrDefault();

        if (DeleteUser != null)
        {
            _context.Set<User>().Remove(DeleteUser);
            await _context.SaveChangesAsync(cancellationToken);
            return DeleteUser;
        }
        else
        {
            return null;
        }
    }
    public async Task<List<UserComm>> GetUserComms(int commsUserId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _context.Set<UserComm>().Where(uc => uc.UserId == commsUserId).ToListAsync();
            return response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<User> UpdateUserPhoto(User user, string userPhoto, CancellationToken cancellationToken)
    {
        user.UserPhoto = userPhoto;
        _context.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserPhone(User user, string mobilerNo, string isd, CancellationToken cancellationToken)
    {
        user.MobileNo = mobilerNo;
        user.Isdcode = isd;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> BadEmail(string email)
    {
        int indexOfAt = email.IndexOf('@');
        string domain = email.Substring(indexOfAt + 1).Trim();
        bool exist = await _context.Set<BadDomain>().Where(w => w.DomainName == domain && w.Status == 1).AnyAsync();
        return exist;
    }

    public async Task<bool> DuplicateEmail(string email)
    {
        return await _context.Set<User>().Where(t => t.PrimaryEmail == email).AnyAsync();
    }

    public async Task<string> SendInvites(CancellationToken cancellationToken)
    {
        var pendingUsers = _context.Set<User>().Where(t => t.Status == 2 && t.CompanyId == companyId).ToList();

        if (pendingUsers.Count() > 0)
        {
            foreach (var usr in pendingUsers)
            {
                _SDE.NewUserAccount(usr.PrimaryEmail, usr.FirstName + " " + usr.LastName, usr.CompanyId, usr.UniqueGuiId);
            }

            return "Invitation sent to all pending users";
        }
        else
        {
            return "No pending users";
        }
    }

    public async Task<IEnumerable<UserDeviceListModel>> GetAllOneUserDeviceList(int quiredUserId, CancellationToken cancellationToken)
    {
        var response = (from UD in _context.Set<UserDevice>()
                        join U in _context.Set<User>() on UD.UserId equals U.UserId
                        where UD.UserId == quiredUserId
                        select new UserDeviceListModel()
                        {
                            UserId = UD.UserId,
                            UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                            UserDeviceID = UD.UserDeviceId,
                            CompanyID = UD.CompanyId,
                            DeviceId = UD.DeviceId,
                            DeviceType = UD.DeviceType,
                            DeviceOs = UD.DeviceOs,
                            DeviceModel = UD.DeviceModel,
                            ExtraInfo = UD.ExtraInfo,
                            Status = UD.Status,
                            DeviceSerial = UD.DeviceSerial,
                            SirenOn = UD.SirenOn,
                            OverrideSilent = UD.OverrideSilent,
                            LastLoginFrom = UD.UpdatedOn
                        }).ToList();
        if (response != null)
        {
            return response;
        }
        throw new UserNotFoundException(companyId, userId);

    }

    public async Task<bool> DeleteUserDevice(int userDeviceId, CancellationToken cancellationToken)
    {
        var userDevice = _context.Set<UserDevice>().Where(ud => ud.UserDeviceId == userDeviceId).FirstOrDefault();
        if (userDevice != null)
        {
            _context.Set<UserDevice>().Remove(userDevice);
            _context.SaveChanges();

            var tracking = _context.Set<TrackMe>().Where(tm => tm.UserDeviceId == userDeviceId).ToList();
            _context.Set<TrackMe>().RemoveRange(tracking);
            _context.SaveChanges();
            return true;
        }
        throw new UserNotFoundException(companyId, userId);
    }

    public async Task<UserRelations> UserRelations(CancellationToken cancellationToken)
    {
        UserRelations UR = new UserRelations();
        try
        {
            bool ShowAllGroups = true;
            bool.TryParse(_DBC.GetCompanyParameter("SHOW_ALL_GROUPS_TO_USERS", companyId), out ShowAllGroups);
            UR.ShowAllGroups = ShowAllGroups;

            if (!ShowAllGroups)
            {
                var userRelations = (from UOR in _context.Set<ObjectRelation>()
                                     join OBM in _context.Set<ObjectMapping>() on UOR.ObjectMappingId equals OBM.ObjectMappingId
                                     join OBJ in _context.Set<Core.Models.Object>() on OBM.SourceObjectId equals OBJ.ObjectId
                                     where UOR.TargetObjectPrimaryId == userId && (OBJ.ObjectTableName == "Group" || OBJ.ObjectTableName == "Location")
                                     select new UGroup
                                     {
                                         ObjectTableName = OBJ.ObjectTableName,
                                         SourceObjectPrimaryId = UOR.SourceObjectPrimaryId,
                                         ObjectMappingId = OBM.ObjectMappingId
                                     }).ToList();
                var newLocation = new Core.Locations.Location();
                newLocation.CompanyId = companyId;
                newLocation.LocationName = "ALL";
                newLocation.Lat = "";
                newLocation.Long = "";
                newLocation.Desc = "All Locations";
                newLocation.PostCode = " ";
                newLocation.Status = 1;
                newLocation.CreatedBy = userId;
                newLocation.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                int newLocaionId = await _locationRepository.CreateLocation(newLocation, cancellationToken);
                var newGroup = new Core.Groups.Group();
                newGroup.CompanyId = companyId;
                newGroup.GroupName = "ALL";
                newGroup.Status = 1;
                newGroup.CreatedBy = userId;
                newGroup.UpdatedBy = userId;
                newGroup.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                newGroup.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                int newGroupId = await _groupRepository.CreateGroup(newGroup, cancellationToken);

                UR.Groups = userRelations;

                List<int> relatedUserId = new List<int>();

                foreach (var relation in userRelations)
                {

                    if ((newLocaionId == relation.SourceObjectPrimaryId && relation.ObjectTableName == "Location") ||
                        newGroupId == relation.SourceObjectPrimaryId && relation.ObjectTableName == "Group")
                    {
                        continue;
                    }

                    var getuser = (from UOR in _context.Set<ObjectRelation>()
                                   where UOR.ObjectMappingId == relation.ObjectMappingId && UOR.SourceObjectPrimaryId == relation.SourceObjectPrimaryId
                                   select UOR).ToList();
                    foreach (var user in getuser)
                    {
                        relatedUserId.Add(user.TargetObjectPrimaryId);
                    }
                }
                UR.Users = relatedUserId.Distinct().ToList();
            }
            return UR;

        }
        catch (Exception ex)
        {
            //ToDO: throw exceptions
            throw ex;
        }
        return UR;
    }

    public void ResetUserDeviceToken(int qUserId)
    {
        try
        {
            var dvcs = _context.Set<UserDevice>().Where(w => w.UserId == qUserId).ToList();
            foreach (var dvc in dvcs)
            {
                dvc.DeviceToken = Guid.NewGuid().ToString();
            }
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
            //ToDo: throw exception
        }
    }

    public void CreateUserSecurityGroup(int userId, int securityGroupId, int createdUpatedBy, int companyId, string securityGroupStandatdFilter = "")
    {
        try
        {
            int newSecurityGroupId = 0;
            if (securityGroupId > 0)
            {
                newSecurityGroupId = securityGroupId;
            }
            else if (!string.IsNullOrEmpty(securityGroupStandatdFilter))
            {
                newSecurityGroupId = _context.Set<SecurityGroup>().Where(t => t.CompanyId == companyId && t.Name == securityGroupStandatdFilter).Select(t => t.SecurityGroupId).FirstOrDefault();
            }
            if (newSecurityGroupId > 0)
            {
                bool isUserSecurityGroupDefined = _context.Set<UserSecurityGroup>().Where(t => t.UserId == userId && t.SecurityGroupId == securityGroupId).Any();
                if (!isUserSecurityGroupDefined)
                {
                    UserSecurityGroup newUserSecurityGroup = new UserSecurityGroup();
                    newUserSecurityGroup.UserId = userId;
                    newUserSecurityGroup.SecurityGroupId = securityGroupId;
                    _context.Set<UserSecurityGroup>().Add(newUserSecurityGroup);
                    _context.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
            //ToDo: throw exception
        }
    }

    public void UserSecurityGroup(int userId, string[] totSecGroup, int currentUserId, int companyId)
    {
        try
        {
            if (totSecGroup.Length > 0)
            {
                var regUserDel = _context.Set<UserSecurityGroup>().Where(t => t.UserId == userId).ToList();
                List<int[]> USGList = new List<int[]>();
                foreach (string group in totSecGroup)
                {
                    int groupid = Convert.ToInt16(group);
                    var ISExist = regUserDel.FirstOrDefault(s => s.UserId == userId && s.SecurityGroupId == groupid);
                    if (ISExist == null)
                    {
                        CreateUserSecurityGroup(userId, Convert.ToInt16(group), currentUserId, companyId);
                        ResetUserDeviceToken(userId);
                    }
                    else
                    {
                        int[] Arr = new int[2];
                        Arr[0] = ISExist.UserId;
                        Arr[1] = ISExist.SecurityGroupId;
                        USGList.Add(Arr);
                    }
                }
                foreach (var item in regUserDel)
                {
                    bool ISDEL = USGList.Any(s => s[0] == item.UserId && s[1] == item.SecurityGroupId);
                    if (!ISDEL)
                    {
                        _context.Set<UserSecurityGroup>().Remove(item);
                    }
                }
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            //ToDo: throw exception
        }
    }

    public void UserObjectRelation(int userId, string[] objFilters, int currentUserId, int companyId, string timeZoneId, string deptAction = "REPLACE", string locAction = "REPLACE")
    {

        try
        {
            List<int> OBJRList = new List<int>();

            List<ObjFltrList> objf = new List<ObjFltrList>();

            if (objFilters.Length > 0)
            {
                foreach (string filter in objFilters)
                {
                    string[] subFilter = filter.Split(',');
                    int delObjmapid = Convert.ToInt32(subFilter[0]);

                    for (int loop = 0; loop < subFilter.Length; loop++)
                    {
                        if (loop == 0)
                        {
                            objf.Add(new ObjFltrList() { ObjMapId = delObjmapid, SourceId = 0 });
                        }
                        else
                        {
                            int sourceid = Convert.ToInt32(subFilter[loop]);
                            objf.Add(new ObjFltrList() { ObjMapId = delObjmapid, SourceId = sourceid });
                        }
                    }
                }

                var UniqOBjs = objf.Select(s => s.ObjMapId).Distinct();

                foreach (int OBjs in UniqOBjs)
                {
                    var objMap = (from OM in _context.Set<ObjectMapping>()
                                  join O in _context.Set<Core.Models.Object>() on OM.SourceObjectId equals O.ObjectId
                                  where OM.ObjectMappingId == OBjs
                                  select O).FirstOrDefault();
                    if (objMap != null)
                    {
                        var ObjName = objMap.ObjectTableName;

                        var SourceIds = objf.Where(s => s.ObjMapId == OBjs && s.SourceId > 0).ToList();
                        var queryRec = _context.Set<ObjectRelation>().Where(t => t.TargetObjectPrimaryId == userId && t.ObjectMappingId == OBjs).ToList();

                        if (SourceIds.Count > 0)
                        {
                            foreach (var SourceId in SourceIds)
                            {
                                var IsExist = queryRec.FirstOrDefault(s => s.TargetObjectPrimaryId == userId && s.SourceObjectPrimaryId == SourceId.SourceId && s.ObjectMappingId == OBjs);
                                if (IsExist == null)
                                {
                                    _DBC.CreateNewObjectRelation(SourceId.SourceId, userId, OBjs, currentUserId, timeZoneId, companyId);
                                }
                                else
                                {
                                    bool IsEdxistdata = OBJRList.Any(s => s == IsExist.ObjectRelationId);
                                    if (!IsEdxistdata)
                                        OBJRList.Add(IsExist.ObjectRelationId);
                                }
                            }
                        }

                        if ((deptAction == "REPLACE" && ObjName == "Group") || (locAction == "REPLACE" && ObjName == "Location"))
                        {
                            foreach (var item in queryRec)
                            {
                                bool ISDEL = OBJRList.Any(s => s == item.ObjectRelationId);
                                if (!ISDEL || SourceIds.Count == 0)
                                {
                                    _context.Set<ObjectRelation>().Remove(item);
                                }
                            }
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<bool> BulkAction(BulkActionModel request, CancellationToken cancellationToken)
    {
        try
        {
            var userList = await _context.Set<User>().Where(user => request.UserList.Contains(user.UserId)).ToListAsync();


            foreach (var user in userList)
            {
                if (request.Action == "DELETE")
                {
                    if (user.RegisteredUser == false)
                    {
                        var userToBeDeleted = new User();

                        await DeleteUser(user, cancellationToken);
                    }
                }
                else if (request.Action == "DEACTIVATE")
                {
                    user.Status = 0;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else if (request.Action == "ACTIVATE")
                {
                    user.Status = 1;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else if (request.Action == "VERIFY")
                {
                    if (user.Status == 2)
                    {
                        user.Status = 1;
                        user.FirstLogin = true;
                        await _context.SaveChangesAsync(cancellationToken);
                        //Assign the user to the default location and depeartment and send the account details
                        _DBC.CreateObjectRelationship(user.UserId, 0, "Location", user.CompanyId, request.CurrentUserId, timeZoneId, "ALL");
                        _DBC.CreateObjectRelationship(user.UserId, 0, "Group", user.CompanyId, request.CurrentUserId, timeZoneId, "ALL");

                        if (request.SendInvite)
                            _SDE.NewUserAccountConfirm(user.PrimaryEmail, user.FirstName + " " + user.LastName, user.Password, user.CompanyId, user.UniqueGuiId);

                    }
                }
                else if (request.Action == "CREDENTIAL")
                {
                    _SDE.NewUserAccountConfirm(user.PrimaryEmail, user.FirstName + " " + user.LastName, user.Password, user.CompanyId, user.UniqueGuiId);
                    user.FirstLogin = true;
                    _context.SaveChanges();
                }
                else if (request.Action == "INVITE")
                {
                    if (user.Status == 2)
                    {
                        _SDE.NewUserAccount(user.PrimaryEmail, user.FirstName + " " + user.LastName, user.CompanyId, user.UniqueGuiId);
                    }
                }
                else if (request.Action == "EDIT")
                {

                    if (user.RegisteredUser == false)
                    {

                        string[] ObjFilters = request.Filters.Split(';');

                        if (user.Status != 2 && request.SetStatus == "1")
                        {
                            user.Status = request.Status;
                        }

                        if (user.UserRole.ToUpper() != request.UserRole.ToUpper() && request.SetUserRole == "1")
                        {
                            //_billing.AddUserRoleChange(CompanyId, user.UserId, InputModel.UserRole.ToUpper(), TimeZoneId);
                            // ToDo: implement billing
                            ResetUserDeviceToken(user.UserId);
                        }

                        if (request.Status == 0 && request.SetStatus == "1")
                            RemoveUserDevice(user.UserId);

                        if (request.SetUserLanguage == "1")
                        {
                            user.UserLanguage = request.UserLanguage;
                        }
                        if (request.SetUserRole == "1")
                        {
                            user.UserRole = request.UserRole.ToUpper();
                        }
                        user.UpdatedBy = request.CurrentUserId;
                        user.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                        _context.SaveChanges();
                        await CreateUserSearch(user.UserId, user.FirstName, user.LastName, user.Isdcode, user.MobileNo, user.PrimaryEmail, companyId);

                        if (request.SetSecurityGroups == "1")
                        {
                            string[] totSecGroup = request.SecurityGroups.Split(',');
                            UserSecurityGroup(user.UserId, totSecGroup, request.CurrentUserId, companyId);
                        }

                        if (request.SetDepartment == "1")
                        {
                            user.DepartmentId = request.Department;
                        }

                        if (request.SetLocation == "1" || request.SetGroup == "1")
                        {
                            UserObjectRelation(user.UserId, ObjFilters, request.CurrentUserId, companyId, timeZoneId, request.GroupActionGroup, request.GroupActionLocation);

                            _DBC.CreateObjectRelationship(user.UserId, 0, "Location", companyId, user.UserId, timeZoneId, "ALL");

                            _DBC.CreateObjectRelationship(user.UserId, 0, "Group", companyId, user.UserId, timeZoneId, "ALL");
                        }

                        if (request.SetPingMethod == "1")
                        {
                            UserCommsMethods(user.UserId, "Ping", request.PingMethod, request.CurrentUserId, companyId, timeZoneId);
                        }

                        if (request.SetIncidentMethod == "1")
                        {
                            UserCommsMethods(user.UserId, "Incident", request.IncidentMethod, request.CurrentUserId, companyId, timeZoneId);
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private bool ValidateUserPassword(int userId, string newPassword, int companyId, bool saveHistory, out string pwdError)
    {
        pwdError = string.Empty;
        bool pwdNotUsed = true;
        newPassword = newPassword.Trim();
        int lastNPwdCheck = Convert.ToInt16(_DBC.GetCompanyParameter("LAST_PASSWORD_CHECK", companyId));
        try
        {
            var pwdHistory = _context.Set<PasswordChangeHistory>().Where(t => t.UserId == userId).ToList();
            var lastXPwd = (from LP in pwdHistory select LP).OrderByDescending(o => o.Id).Take(lastNPwdCheck);

            foreach (var pwd in lastXPwd)
            {
                if (pwd.LastPassword.Trim() == newPassword)
                {
                    pwdNotUsed = false;
                }
            }

            if (!pwdNotUsed)
            {
                pwdError = "New password cannot be the past " + lastNPwdCheck + " used passwords";
            }
            else
            {
                if (saveHistory)
                {
                    int ChngId = _DBC.AddPwdChangeHistory(userId, newPassword);
                }
            }

            var delPwd = _context.Set<PasswordChangeHistory>().OrderByDescending(t => t.Id).Skip(lastNPwdCheck).ToList();
            if (delPwd.Count > 0)
            {
                _context.Set<PasswordChangeHistory>().RemoveRange(delPwd);
                _context.SaveChanges();
            }


        }
        catch (Exception ex)
        {
            return false;
        }
        return pwdNotUsed;
    }
    public async Task<List<UserGroup>> GetUserGroups(int userId)
    {
        try
        {

            var pUserId = new SqlParameter("@UserID", userId);

            var result = await _context.Set<UserGroup>().FromSqlRaw("EXEC Get_User_Object_Relation @UserID", pUserId).ToListAsync();
            return result;

        }
        catch (Exception ex)
        {
            throw new GroupUserNotFound(companyId, userId);
        }
    }

    public async Task<dynamic> OffDuty(OffDutyModel request, CancellationToken cancellationToken)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {
            bool oncall_exist = false;
            var oncall = await _context.Set<OffDuty>().Where(t => t.UserId == userId).FirstOrDefaultAsync();
            if (oncall != null)
                oncall_exist = true;

            if (request.OffDutyAction.ToUpper() == "START")
            {
                if (oncall != null)
                {
                    RTO.ErrorCode = "E208";
                    RTO.ErrorId = 208;
                    RTO.Status = true;
                    RTO.Message = "On Call already activated";
                    return RTO;
                }
            }
            else if (request.OffDutyAction.ToUpper() == "END" || request.OffDutyAction.ToUpper() == "CHECK")
            {
                if (oncall == null)
                {
                    RTO.ErrorCode = "E209";
                    RTO.ErrorId = 209;
                    RTO.Status = true;
                    RTO.Message = "No active on call record is found";
                    return RTO;
                }
            }
            bool notifyFront = true;
            if (oncall_exist)
            {
                DateTime CurrentDateTime = oncall.EndDateTime.LocalDateTime;
                if (CurrentDateTime < DateTime.Now)
                {
                    request.OffDutyAction = "END";
                    notifyFront = false;
                }
            }

            if (request.OffDutyAction.ToUpper() == "CHECK" && oncall_exist && notifyFront)
            {
                return OffDutyCheck(oncall);
            }
            else if (request.OffDutyAction.ToUpper() == "START" && !oncall_exist)
            {
                return OffDutyStart(request, cancellationToken);
            }
            else if (request.OffDutyAction.ToUpper() == "CHANGE" && oncall_exist)
            {
                return OffDutyChange(request, cancellationToken);
            }
            else if (request.OffDutyAction.ToUpper() == "END" && oncall_exist)
            {
                return OffDutyEnd(request, notifyFront, cancellationToken);
            }
            else
            {
                RTO.ErrorCode = "E209";
                RTO.ErrorId = 209;
                RTO.Status = true;
                RTO.Message = "No active on call record is found";
            }

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return null;
    }

    public dynamic OffDutyCheck(OffDuty oncall)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {
            if (oncall != null)
            {
                RTO.Data = oncall;
                RTO.Status = true;
            }
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return RTO;
    }

    public async Task<dynamic> OffDutyStart(OffDutyModel request, CancellationToken cancellationToken)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {
            var user = await _context.Set<User>().Where(t => t.UserId == userId && t.Status == 1).FirstOrDefaultAsync();
            if (user != null)
            {
                request.AllowEmail = request.Source == "APP" ? true : request.AllowEmail;

                OffDuty oc = new OffDuty();
                oc.ActivationSource = request.Source;
                oc.AllowEmail = request.AllowEmail;
                oc.AllowPhone = request.AllowPhone;
                oc.AllowPush = request.AllowPush;
                oc.AllowText = request.AllowText;
                oc.StartDateTime = _DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime);
                oc.EndDateTime = _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime);
                oc.UserId = userId;

                await _context.AddAsync(oc, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                if (_DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime) <= _DBC.GetDateTimeOffset(DateTime.Now) &&
                    _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime) >= _DBC.GetDateTimeOffset(DateTime.Now))
                {
                    user.ActiveOffDuty = 1;

                    if (request.AllowEmail || request.AllowPhone || request.AllowPush || request.AllowText)
                    {
                        user.ActiveOffDuty = 2;
                    }
                }
                else
                {
                    user.ActiveOffDuty = 0;
                    CreateOffDutyJob(userId, _DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime), companyId, "START");
                }

                CreateOffDutyJob(userId, _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime),companyId, "END");

                _context.SaveChangesAsync();

                RTO.Data = oc;
                RTO.Status = true;
            }
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return RTO;
    }

    public async Task<List<UserParams>> GetUserSystemInfo(int userId, int companyId)
    {
        List<UserParams> uparams = new List<UserParams>();
        try
        {
            var pUserId = new SqlParameter("@UserID", userId);
            var pCompanyID = new SqlParameter("@CompanyID", companyId);

            uparams = await _context.Set<UserParams>().FromSqlRaw("EXEC Pro_Get_User_System_Info @UserID, @CompanyID", pUserId, pCompanyID).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return uparams;
    }

    public async Task<dynamic> OffDutyChange(OffDutyModel request, CancellationToken cancellationToken)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {
            var user = await _context.Set<User>().Where(u => u.UserId == userId && u.Status == 1).FirstOrDefaultAsync();
            if (user != null)
            {
                user.ActiveOffDuty = 0;
                await _context.SaveChangesAsync();

                CreateOffDutyHistory(userId, companyId, cancellationToken);

                return OffDutyStart(request, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return RTO;
    }

    public async Task<dynamic> OffDutyEnd(OffDutyModel request, bool notifyFront, CancellationToken cancellationToken)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {
            var user = await _context.Set<User>().Where(u => u.UserId == userId && u.Status == 1).FirstOrDefaultAsync();
            if (user != null)
            {
                CreateOffDutyHistory(userId, companyId, cancellationToken);

                if (request.OffDutyAction.ToUpper() == "END")
                {
                    var alloncall = await _context.Set<OffDuty>().Where(t => t.UserId == userId).OrderByDescending(t => t.OffDutyId).ToListAsync();
                    _context.Set<OffDuty>().RemoveRange(alloncall);
                    await _context.SaveChangesAsync();
                }

                user.ActiveOffDuty = 0;
                await _context.SaveChangesAsync();

                _DBC.DeleteScheduledJob("OFF_DUTY_END_" +userId, "OFF_DUTY_JOB_END");

                if (notifyFront == true)
                {
                    RTO.ErrorCode = "E210";
                    RTO.ErrorId = 210;
                    RTO.Message = "On Call has been deactivated successfully";
                    RTO.Status = true;
                }
                else
                {
                    RTO.ErrorCode = "E209";
                    RTO.ErrorId = 209;
                    RTO.Message = "No active on call record is found";
                    RTO.Status = true;
                    RTO.Data = null;
                }
            }

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return RTO;
    }

    private async void CreateOffDutyHistory(int userId, int companyId, CancellationToken cancellationToken)
    {
        try
        {
            var oncall = await _context.Set<OffDuty>().Where(t => t.UserId == userId).OrderByDescending(t => t.OffDutyId).FirstOrDefaultAsync();
            if (oncall != null)
            {
                OffDutyHistory OCH = new OffDutyHistory();
                OCH.ActivationSource = oncall.ActivationSource;
                OCH.AllowEmail = oncall.AllowEmail;
                OCH.AllowPhone = oncall.AllowPhone;
                OCH.AllowPush = oncall.AllowPush;
                OCH.AllowText = oncall.AllowText;
                OCH.StartDateTime = oncall.StartDateTime;
                OCH.EndDateTime = oncall.EndDateTime;
                OCH.UserId = oncall.UserId;
                _context.Set<OffDutyHistory>().Add(OCH);
                _context.Set<OffDuty>().Remove(oncall);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<User> GetUserId(int companyId, string email)
    {
        try
        {

            var result = await _context.Set<User>().Where(t => t.PrimaryEmail == email.ToLower()).FirstOrDefaultAsync();
            if (result != null)
            {
                if (result.CompanyId == companyId)
                {
                    return new User { UserId = result.UserId };
                }
                else
                {
                    return new User { UserId = -1 };
                }
            }
            else
            {
                return new User { UserId = 0 };
            }

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<dynamic> OffDutyActivate(OffDutyModel request, bool exist, CancellationToken cancellationToken)
    {
        OffDutyResponse RTO = new OffDutyResponse();
        try
        {

            string TimeZoneId =await _DBC.GetTimeZoneVal(userId);
            var user = await _context.Set<User>().Where(t => t.UserId == userId && t.Status == 1).FirstOrDefaultAsync();

            if (request.OffDutyAction.ToUpper() == "END" || request.OffDutyAction.ToUpper() == "CHANGE" || request.OffDutyAction.ToUpper() == "CHECK")
            {
                var oncall = await _context.Set<OffDuty>().Where(t => t.UserId == userId).OrderByDescending(t => t.OffDutyId).FirstOrDefaultAsync();

                bool NotifyFront = true;

                if (oncall != null)
                {

                    DateTime CurrentDateTime = oncall.EndDateTime.LocalDateTime;
                    if (CurrentDateTime < DateTime.Now)
                    {
                        user.ActiveOffDuty = 0;
                        request.OffDutyAction = "END";
                        NotifyFront = false;
                    }

                    if (request.OffDutyAction.ToUpper() == "END" || request.OffDutyAction.ToUpper() == "CHANGE")
                    {
                        OffDutyHistory OCH = new OffDutyHistory();
                        OCH.ActivationSource = oncall.ActivationSource;
                        OCH.AllowEmail = oncall.AllowEmail;
                        OCH.AllowPhone = oncall.AllowPhone;
                        OCH.AllowPush = oncall.AllowPush;
                        OCH.AllowText = oncall.AllowText;
                        OCH.StartDateTime = oncall.StartDateTime;
                        OCH.EndDateTime = oncall.EndDateTime;
                        OCH.UserId = oncall.UserId;
                        await _context.Set<OffDutyHistory>().AddAsync(OCH);
                        _context.Set<OffDuty>().Remove(oncall);
                        user.ActiveOffDuty = 0;
                        await _context.SaveChangesAsync(cancellationToken);

                        RTO.ErrorCode = "E210";
                        RTO.ErrorId = 210;
                        RTO.Message = "On Call has been deactivated successfully";
                        RTO.Status = true;

                        if (request.OffDutyAction.ToUpper() == "END")
                        {
                            var alloncall = await _context.Set<OffDuty>().Where(t => t.UserId == userId).OrderByDescending(t => t.OffDutyId).ToListAsync();
                            _context.Set<OffDuty>().RemoveRange(alloncall);
                            await _context.SaveChangesAsync(cancellationToken);
                        }

                        _DBC.DeleteScheduledJob("OFF_DUTY_END_" + userId, "OFF_DUTY_JOB_END");
                        _DBC.DeleteScheduledJob("OFF_DUTY_START_" +userId, "OFF_DUTY_JOB_START");

                        if (request.OffDutyAction.ToUpper() == "END" && NotifyFront == true)
                        {
                            return RTO;
                        }
                    }

                    if (request.OffDutyAction.ToUpper() == "CHECK" && NotifyFront == true)
                    {
                        RTO.Data = oncall;
                        RTO.Status = true;
                        return RTO;
                    }

                }

                if (request.OffDutyAction.ToUpper() != "CHANGE" || (NotifyFront == false && request.OffDutyAction.ToUpper() == "END"))
                {
                    RTO.ErrorCode = "E209";
                    RTO.ErrorId = 209;
                    RTO.Message = "No active on call record is found";
                    RTO.Status = true;
                    RTO.Data = null;
                    return RTO;
                }
            }

            request.AllowEmail = request.Source == "APP" ? true : request.AllowEmail;
            DateTimeOffset SrvStartDateTime = request.StartDateTime;
            DateTimeOffset SrvEndDateTime = request.EndDateTime;

            if ((!exist && request.OffDutyAction.ToUpper() == "START") || request.OffDutyAction.ToUpper() == "CHANGE")
            {

                OffDuty OC = new OffDuty();
                OC.ActivationSource = request.Source;
                OC.AllowEmail = request.AllowEmail;
                OC.AllowPhone = request.AllowPhone;
                OC.AllowPush = request.AllowPush;
                OC.AllowText = request.AllowText;
                OC.StartDateTime = _DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime);
                OC.EndDateTime = _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime);
                OC.UserId = userId;
                await _context.Set<OffDuty>().AddAsync(OC);

                if (_DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime) <= _DBC.GetDateTimeOffset(DateTime.Now) &&
                    _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime) >= _DBC.GetDateTimeOffset(DateTime.Now))
                {
                    user.ActiveOffDuty = 1;

                    if (request.AllowEmail || request.AllowPhone || request.AllowPush || request.AllowText)
                    {
                        user.ActiveOffDuty = 2;
                    }
                }
                else
                {
                    user.ActiveOffDuty = 0;
                    CreateOffDutyJob(userId, _DBC.ConvertToLocalTime("GMT Standard Time", request.StartDateTime), companyId, "START");
                }

                CreateOffDutyJob(userId, _DBC.ConvertToLocalTime("GMT Standard Time", request.EndDateTime), companyId, "END");
                await _context.SaveChangesAsync(cancellationToken);

                RTO.Data = OC;
                RTO.Status = true;
            }
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return RTO;
    }

    public async Task<dynamic> UpdateGroupMember(int targetId, int userId, int objMapId, string action, int currentUserId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        try
        {
            if (action.ToUpper() == "ADD" && objMapId > 0)
            {
                _DBC.CreateNewObjectRelation(targetId, userId, objMapId, currentUserId, timeZoneId, companyId);
            }
            else if (objMapId > 0)
            {
                var delObjs = await _context.Set<ObjectRelation>().Where(t =>
                                t.ObjectMappingId == objMapId &&
                                t.SourceObjectPrimaryId == targetId &&
                                t.TargetObjectPrimaryId == userId
                                ).FirstOrDefaultAsync();
                if (delObjs != null)
                {
                    _context.Set<ObjectRelation>().Remove(delObjs);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                await UpdateUserDepartment(userId, targetId, action, currentUserId, companyId, timeZoneId);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public void CreateOffDutyJob(int userId, DateTimeOffset offDutyDate, int companyId, string action = "START", string timeZoneId = "GMT Standard Time")
    {
        try
        {
            //string job_name = "OFF_DUTY_" + action + "_" + userId;

            ////DBC.DeleteScheduledJob(job_name, "OFF_DUTY_JOB_" + Action);

            //ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
            //IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

            //var jobDetail = new Quartz.Impl.JobDetailImpl(job_name, "OFF_DUTY_JOB_" + Action, typeof(OffDutyJob));
            //jobDetail.JobDataMap["UserID"] = UserID;
            //jobDetail.JobDataMap["Action"] = Action;
            //jobDetail.JobDataMap["CompanyID"] = CompanyID;

            //string CronExpressionStr = "0 " + OffDutyDate.Minute + " " + OffDutyDate.Hour + " " + OffDutyDate.Day + " " + OffDutyDate.Month + " ? " + OffDutyDate.Year;

            //var trigger = TriggerBuilder.Create()
            //                             .WithIdentity(job_name, "OFF_DUTY_JOB_" + Action)
            //                             .WithCronSchedule(CronExpressionStr,
            //                              x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId))
            //                             .WithMisfireHandlingInstructionDoNothing()
            //                             ).ForJob(jobDetail)
            //                             .Build();

            //var run = _scheduler.ScheduleJob(jobDetail, trigger);
            //_DBC.CreateLog("INFO", action + trigger.GetNextFireTimeUtc().ToString());

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<int> CreateUsers(int CompanyId, bool RegisteredUser, string FirstName, string PrimaryEmail, string Password,
        int Status, int CreatedUpdatedBy, string TimeZoneId, string LastName = "", string MobileNo = "", string UserRole = "",
        string UserPhoto = "no-photo.jpg", string ISDCode = "", string LLIsdCode = "", string LandLine = "", string SecondaryEmail = "",
        string UniqueGuiD = "", string Lat = "", string Lng = "", string Token = "", bool ExpirePassword = true, string UserLanguage = "en",
        bool SMSTrigger = false, bool FirstLogin = true, int DepartmentId = 0)
    {
        try
        {

            bool check_Email = await DuplicateEmail(PrimaryEmail);
            if (check_Email)
                return 0;

            User NewUsers = new User();

            NewUsers.CompanyId = CompanyId;
            NewUsers.RegisteredUser = RegisteredUser;
            NewUsers.FirstName = FirstName;

            if (!string.IsNullOrEmpty(LastName))
                NewUsers.LastName = LastName;

            if (!string.IsNullOrEmpty(ISDCode))
            {
                NewUsers.Isdcode = _DBC.Left(ISDCode, 1) != "+" ? "+" + ISDCode : ISDCode;
            }

            if (!string.IsNullOrEmpty(MobileNo))
                NewUsers.MobileNo = _DBC.FixMobileZero(MobileNo);

            if (!string.IsNullOrEmpty(LLIsdCode))
                NewUsers.Llisdcode = _DBC.Left(LLIsdCode, 1) != "+" ? "+" + LLIsdCode : LLIsdCode;

            if (!string.IsNullOrEmpty(LandLine))
                NewUsers.Landline = _DBC.FixMobileZero(LandLine);

            NewUsers.PrimaryEmail = PrimaryEmail.ToLower();
            NewUsers.UserHash = _DBC.PWDencrypt(PrimaryEmail.ToLower());

            if (!string.IsNullOrEmpty(SecondaryEmail))
                NewUsers.SecondaryEmail = SecondaryEmail;

            NewUsers.Password = Password;

            if (!string.IsNullOrEmpty(UniqueGuiD))
                NewUsers.UniqueGuiId = UniqueGuiD;
            else
                NewUsers.UniqueGuiId = Guid.NewGuid().ToString();

            NewUsers.Status = Status;

            if (!string.IsNullOrEmpty(UserPhoto))
                NewUsers.UserPhoto = UserPhoto;

            if (!string.IsNullOrEmpty(UserRole))
            {
                NewUsers.UserRole = UserRole.ToUpper().Replace("STAFF", "USER");
            }
            else
            {
                NewUsers.UserRole = "USER";
            }

            if (!string.IsNullOrEmpty(Lat))
                NewUsers.Lat = _DBC.Left(Lat, 15);

            if (!string.IsNullOrEmpty(Lng))
                NewUsers.Lng = _DBC.Left(Lng, 15);

            string CompExpirePwd = _DBC.GetCompanyParameter("EXPIRE_PASSWORD", CompanyId);

            if (CompExpirePwd == "true")
            {
                NewUsers.ExpirePassword = ExpirePassword;
            }
            else
            {
                NewUsers.ExpirePassword = false;
            }

            NewUsers.UserLanguage = UserLanguage;
            NewUsers.PasswordChangeDate = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
            NewUsers.FirstLogin = FirstLogin;

            NewUsers.CreatedBy = CreatedUpdatedBy;
            NewUsers.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
            NewUsers.UpdatedBy = CreatedUpdatedBy;
            NewUsers.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
            NewUsers.TrackingStartTime = SqlDateTime.MinValue.Value;
            NewUsers.TrackingEndTime = SqlDateTime.MinValue.Value;
            NewUsers.LastLocationUpdate = SqlDateTime.MinValue.Value;
            NewUsers.DepartmentId = DepartmentId;
            NewUsers.Otpexpiry = SqlDateTime.MinValue.Value;

            var roles = _DBC.CCRoles(true);
            NewUsers.Smstrigger = (roles.Contains(NewUsers.UserRole.ToUpper()) ? SMSTrigger : false);

            await _context.Set<User>().AddAsync(NewUsers);
            await _context.SaveChangesAsync();

            if (CreatedUpdatedBy <= 0)
            {
                var usr = await _context.Set<User>().Where(t => t.UserId == NewUsers.UserId).FirstOrDefaultAsync();
                usr.CreatedBy = NewUsers.UserId;
                usr.UpdatedBy = NewUsers.UserId;
                await _context.SaveChangesAsync();
            }

            AddPwdChangeHistory(NewUsers.UserId, Password, TimeZoneId);

            await CreateUserSearch(NewUsers.UserId, FirstName, LastName, ISDCode, MobileNo, PrimaryEmail, CompanyId);

            CreateSMSTriggerRight(CompanyId, NewUsers.UserId, NewUsers.UserRole, SMSTrigger, NewUsers.Isdcode, NewUsers.MobileNo);

            return NewUsers.UserId;
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return 0;
    }

    public async Task<dynamic> DeleteUser(int userId, int companyId, int currentUserId, string timeZoneId, CancellationToken cancellationToken)
    {

        var tblUser = await _context.Set<User>().Where(t=>t.CompanyId == companyId && t.UserId == userId).FirstOrDefaultAsync();

        if (tblUser != null)
        {
            if (tblUser.RegisteredUser != true)
            {
                tblUser.Status = 3;
                //tblUser.TOKEN = "";
                tblUser.PrimaryEmail = "DEL-" + tblUser.PrimaryEmail;
                tblUser.UserHash = _DBC.PWDencrypt(tblUser.PrimaryEmail);
                tblUser.UpdatedBy = userId;
                tblUser.UpdatedOn = _DBC.GetLocalTime(timeZoneId, System.DateTime.Now);
               await _context.SaveChangesAsync(cancellationToken);

                await CheckUserAssociation(userId, companyId);

                //Remove all user devices;
                RemoveUserDevice(userId);
                DeleteUsercoms(userId, 0, true);

                //_billing.AddUserRoleChange(CompanyId, UserId, tblUser.UserRole, TimeZoneId);
                // Todo: billing should be added here
            }
            return tblUser.UserId;
        }
        return 0;
    }

    public async Task<bool> CheckUserAssociation(int userId, int companyId)
    {
        try
        {
            bool sendemail = false;

            var pCompanyId = new SqlParameter("@CompanyId", companyId);
            var pUserId = new SqlParameter("@UserID", userId);

            var result =await _context.Set<ModuleLinks>().FromSqlRaw("EXEC Pro_Get_User_Association @UserID, @CompanyID", pUserId, pCompanyId).ToListAsync();

            if (result.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<table width=\"100%\" class=\"user_list\" style=\"border:1px solid #000000;border-collapse: collapse\"><tr width=\"25%\"><th style=\"text-align:left;\">Module</th><th width=\"75%\" style=\"text-align:left;\">Item Name</th></tr>");
                sendemail = true;
                foreach (var item in result)
                {
                    sb.AppendLine("<tr><td>" + item.Module + "</td><td>" + item.ModuleItem + "</td></tr>");
                }
                sb.AppendLine("</table>");
                _SDE.SendUserAssociationsToAdmin(sb.ToString(), userId, companyId);
            }

            return sendemail;
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return false;
    }

    public async void UpdateUserComms(int companyId, int userId, int createdUpdatedBy, string timeZoneId = "GMT Standard Time", string pingMethods = "", string incidentMethods = "", bool isNewUser = false, CancellationToken cancellationToken = default)
    {
        try
        {

            List<string> ImpPingMethods = pingMethods.Split(',').Select(p => p.Trim().ToUpper()).ToList();
            List<string> ImpInciMethods = incidentMethods.Split(',').Select(p => p.Trim().ToUpper()).ToList();

            if (isNewUser && (ImpPingMethods.Count <= 0 || ImpInciMethods.Count <= 0))
            {
                var comms = await (from CC in _context.Set<CompanyComm>()
                             join CM in _context.Set<CommsMethod>() on CC.MethodId equals CM.CommsMethodId
                             where CC.CompanyId == companyId
                             select CM.MethodCode).ToListAsync();
                if (ImpPingMethods.Count <= 0)
                {
                    ImpPingMethods = comms.ToList();
                }

                if (ImpInciMethods.Count <= 0)
                {
                    ImpInciMethods = comms.ToList();
                }
            }

            if (ImpPingMethods.Count > 0)
            {
                ImportUsercomms(companyId, "Ping", userId, ImpPingMethods, createdUpdatedBy, timeZoneId, pingMethods, cancellationToken);
            }

            if (ImpInciMethods.Count > 0)
            {
                ImportUsercomms(companyId, "Incident", userId, ImpInciMethods, createdUpdatedBy, timeZoneId, incidentMethods, cancellationToken);
            }

        }
        catch (Exception ex) {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async void ImportUsercomms(int companyId, string messageType, int userId, List<string> methodList, int createdUpdatedBy, string timeZoneId, string rawMethodsList, CancellationToken cancellationToken)
    {
        try
        {
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
            if (incMethods.Count <= 0 && (methodList.Count <= 0 && !string.IsNullOrEmpty(rawMethodsList)))
            {
                foreach (var comMethod in CompanyCommsMethodid)
                {
                    CreateUserComms(userId, companyId, comMethod.MethodId, createdUpdatedBy, timeZoneId, messageType);
                }
            }
            else if (methodList.Count > 0 && !string.IsNullOrEmpty(rawMethodsList))
            {
                if (incMethods.Count > 0)
                {
                    _context.Set<UserComm>().RemoveRange(incMethods.Select(s => s.UC).ToList());
                    await _context.SaveChangesAsync(cancellationToken);
                }
                var method = (from m in CompanyCommsMethodid where methodList.Contains(m.MethodCode) select m).ToList();
                foreach (var comMethod in method)
                {
                    CreateUserComms(userId, companyId, comMethod.MethodId, createdUpdatedBy, timeZoneId, messageType);
                }
            }

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }



    }

    public async Task<UserRelations> UserRelations(int userId, int companyId, string timeZoneId, CancellationToken cancellationToken)
    {
        UserRelations UR = new UserRelations();
        try
        {
            bool showAllGroups = true;
            bool.TryParse(_DBC.GetCompanyParameter("SHOW_ALL_GROUPS_TO_USERS", companyId), out showAllGroups);
            UR.ShowAllGroups = showAllGroups;

            if (!showAllGroups)
            {
                var userRelations = await (from UOR in _context.Set<ObjectRelation>()
                                     join OBM in _context.Set<ObjectMapping>() on UOR.ObjectMappingId equals OBM.ObjectMappingId
                                     join OBJ in _context.Set<Core.Models.Object>() on OBM.SourceObjectId equals OBJ.ObjectId
                                     where UOR.TargetObjectPrimaryId == userId && (OBJ.ObjectTableName == "Group" || OBJ.ObjectTableName == "Location")
                                     select new UGroup
                                     {
                                         ObjectTableName = OBJ.ObjectTableName,
                                         SourceObjectPrimaryId = UOR.SourceObjectPrimaryId,
                                         ObjectMappingId = OBM.ObjectMappingId
                                     }).ToListAsync();
                Core.Locations.Location newLocation = new Core.Locations.Location();
                newLocation.CompanyId = companyId;
                newLocation.LocationName = "ALL";
                newLocation.Lat = "";
                newLocation.Long = "";
                newLocation.Desc = "All Locations";
                newLocation.PostCode = "";
                newLocation.Status = 1;
                newLocation.CreatedBy = userId;
                int newLocaionId = await _locationRepository.CreateLocation(newLocation, cancellationToken);
                Core.Groups.Group newGroup = new Core.Groups.Group();
                newGroup.CompanyId = companyId;
                newGroup.GroupName = "ALL";
                newGroup.Status = 1;
                newGroup.CreatedBy = userId;
                int newGroupId = await _groupRepository.CreateGroup(newGroup, cancellationToken);

                UR.Groups = userRelations;

                List<int> relatedUserId = new List<int>();

                foreach (var relation in userRelations)
                {

                    if ((newLocaionId == relation.SourceObjectPrimaryId && relation.ObjectTableName == "Location") ||
                        newGroupId == relation.SourceObjectPrimaryId && relation.ObjectTableName == "Group")
                    {
                        continue;
                    }

                    var getUser = await _context.Set<ObjectRelation>().Where(t => t.ObjectMappingId == relation.ObjectMappingId && t.SourceObjectPrimaryId == relation.SourceObjectPrimaryId).ToListAsync();
                    foreach (var user in getUser)
                    {
                        relatedUserId.Add(user.TargetObjectPrimaryId);
                    }
                }
                UR.Users = relatedUserId.Distinct().ToList();
            }
            return UR;

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
        return UR;
    }

    private dynamic _get_member_list(string Sp_Name, int UserID, int CompanyId, int ObjMapID, int TargetID, int RecordStart, int RecordLength, string SearchString,
         string OrderBy, string OrderDir, bool ActiveOnly, string CompanyKey)
    {
        try
        {
            var pCompanyId = new SqlParameter("@CompanyID", CompanyId);
            var pUserID = new SqlParameter("@UserID", UserID);
            var pObjMapID = new SqlParameter("@ObjMapID", ObjMapID);
            var pTargetID = new SqlParameter("@TargetID", TargetID);
            var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
            var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
            var pSearchString = new SqlParameter("@SearchString", SearchString);
            var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
            var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
            var pActiveOnly = new SqlParameter("@ActiveOnly", ActiveOnly);
            var pUniqueKey = new SqlParameter("@UniqueKey", CompanyKey);

            var MainUserlist = new List<MemberUser>();
            var propertyInfo = typeof(MemberUser).GetProperty(OrderBy);

            if (OrderDir == "desc" && propertyInfo != null)
            {
                MainUserlist = _context.Set<MemberUser>().FromSqlRaw(Sp_Name + " @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                       pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                    .OrderByDescending(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else if (OrderDir == "asc" && propertyInfo != null)
            {
                MainUserlist = _context.Set<MemberUser>().FromSqlRaw(Sp_Name + " @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                       pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    })
                    .OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else
            {
                MainUserlist = _context.Set<MemberUser>().FromSqlRaw(Sp_Name + " @CompanyID, @UserID, @ObjMapID, @TargetID, @RecordStart,@RecordLength,@SearchString,@OrderBy,@OrderDir,@ActiveOnly,@UniqueKey",
                       pCompanyId, pUserID, pObjMapID, pTargetID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pActiveOnly, pUniqueKey)
                    .ToList().Select(c => {
                        c.UserFullName = new UserFullName { Firstname = c.FirstName, Lastname = c.LastName };
                        c.PrimaryEmail = c.UserEmail;
                        return c;
                    }).ToList();
            }

            return MainUserlist;
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<dynamic> GetUserDashboard(string modulePage, int userId, bool reverse = false)
    {
        try
        {
            var pModulePage = new SqlParameter("@ModulePage", modulePage);
            var pUserID = new SqlParameter("@UserID", userId);
            var pReverse = new SqlParameter("@Reverse", reverse);

            var result = await _context.Set<DashboardModule>().FromSqlRaw("EXEC Pro_Get_User_Dashboard @ModulePage, @UserID, @Reverse", pModulePage, pUserID, pReverse).ToListAsync();
            return result;

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<dynamic> SaveDashboard(List<DashboardModule> moduleItems, string modulePage, int userId, CancellationToken cancellationToken)
    {
        try
        {
            var olditems = (from UD in _context.Set<UserModuleLink>()
                            join DM in _context.Set<DashboardModule>() on UD.ModuleId equals DM.ModuleId
                            where UD.UserId == userId && DM.ModulePage == modulePage
                            select UD).ToList();
            if (olditems.Count > 0)
            {
                _context.Set<UserModuleLink>().RemoveRange(olditems);
                await _context.SaveChangesAsync();
            }

            if (moduleItems.Count > 0)
            {
                foreach (var item in moduleItems)
                {
                    AddUserModuleItem(userId, item.ModuleId, item.Xpos, item.Ypos, item.Width, item.Height, cancellationToken);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async void AddUserModuleItem(int userId, int moduleId, decimal xPos, decimal yPos, decimal width, decimal height, CancellationToken cancellationToken)
    {
        try
        {
            UserModuleLink UM = new UserModuleLink();
            UM.UserId = userId;
            UM.ModuleId = moduleId;
            UM.Xpos = xPos;
            UM.Ypos = yPos;
            UM.Width = width;
            UM.Height = height;
            await _context.Set<UserModuleLink>().AddAsync(UM);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<dynamic> AddDashlet(int moduleId, int userId, decimal xPos, decimal yPos)
    {
        try
        {
            var pModuleID = new SqlParameter("@ModuleID", moduleId);
            var pUserID = new SqlParameter("@UserID", userId);
            var pXPos = new SqlParameter("@XPos", xPos);
            var pYPos = new SqlParameter("@YPos", yPos);

            var result = await _context.Set<DashboardModule>().FromSqlRaw("EXEC Pro_Add_Dashlet @ModuleID, @UserID,@XPos, @YPos", pModuleID, pUserID, pXPos, pYPos).FirstOrDefaultAsync();
            return result;

        }
        catch (Exception ex)
        {
            throw new UserNotFoundException(companyId, userId);
        }
    }

    public async Task<List<KeyHolderResponse>> GetKeyHolders(int OutUserCompanyId)
    {
        var roles = _DBC.CCRoles(true);
        var kc = await (from U in _context.Set<User>()
                  where U.CompanyId == OutUserCompanyId &&
                  U.Status == 1 &&
                  (roles.Contains(U.UserRole))
                  select new KeyHolderResponse() { UserId = U.UserId, UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName } }).ToListAsync();

        if (kc != null)
        {
            return kc;
        }
        return new List<KeyHolderResponse>();
    }
}
