﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility;
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
    private int userID;
    private int companyID;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        userID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
        companyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
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

    public void CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
        string primaryEmail, int companyId)
    {
        var searchString = firstName + " " + lastName + "|" + primaryEmail + "|" + isdCode + mobileNo;

        var comp = _context.Set<Company>().FirstOrDefault(x => x.CompanyId == companyId);
        if (comp != null)
        {
            var memberUser = _context.Set<MemberUser>().FromSqlRaw("Pro_Create_User_Search {0}, {1}, {2}",
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

    private string Left(string str, int lngth, int stpoint = 0)
    {
        return str.Substring(stpoint, Math.Min(str.Length, lngth));
    }

    private string GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "")
    {
        try
        {
            Key = Key.ToUpper();

            if (CompanyId > 0)
            {
                var LKP = (from CP in _context.Set<CompanyParameter>()
                           where CP.Name == Key && CP.CompanyId == CompanyId
                           select CP).FirstOrDefault();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                else
                {
                    var LPR = (from CP in _context.Set<LibCompanyParameter>()
                               where CP.Name == Key
                               select CP).FirstOrDefault();
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
                var cmp = _context.Set<Company>().Where(w => w.CustomerId == CustomerId).FirstOrDefault();
                if (cmp != null)
                {
                    var LKP = (from CP in _context.Set<CompanyParameter>()
                               where CP.Name == Key && CP.CompanyId == cmp.CompanyId
                               select CP).FirstOrDefault();
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

   
    public async Task<List<MemberUser>> MembershipList(int ObjMapID, MemberShipType memberShipType, int TargetID, int? Start, int? Length, string? Search, List<Order>? order, bool ActiveOnly, string? CompanyKey)
    {
        try
        {
            var RecordStart = Start == 0 ? 0 : Start;
            var RecordLength = Length == 0 ? int.MaxValue : Length;
            var SearchString = (Search != null) ? Search : string.Empty;
            string OrderBy = order != null ? order.FirstOrDefault().column : "FirstName";
            string OrderDir = order != null ? order.FirstOrDefault().dir : "asc";


            var pCompanyId = new SqlParameter("@CompanyID", companyID);
            var pUserID = new SqlParameter("@UserID", userID);
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
            if (memberShipType.ToMemString().ToUpper() == MemberShipType.NON_MEMBER.ToMemString().ToUpper())
            {
                if (OrderDir == "desc" && propertyInfo != null)
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
                else if (OrderDir == "asc" && propertyInfo != null)
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
                if (OrderDir == "desc" && propertyInfo != null)
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
                else if (OrderDir == "asc" && propertyInfo != null)
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
            _logger.LogError("Error occured while trying to seed from the database {0},{1},{2}", ex.Message, ex.StackTrace, ex.InnerException);
            return null;
        }
    }

    
}