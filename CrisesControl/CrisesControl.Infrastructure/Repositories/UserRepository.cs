﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserModel = CrisesControl.Core.Models.EmptyUser;

namespace CrisesControl.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CrisesControlContext _context;
    private readonly int UserID = 16;
    private readonly int CompanyID = 7;
    private readonly string TimeZoneId = "GMT Standard Time";

    public UserRepository(CrisesControlContext context)
    {
        _context = context;
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

    public async Task<IEnumerable<User>> GetAllUsers(int companyId)
    {
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

    public LoginInfoReturnModel GetLoggedInUserInfo(LoginInfo request, CancellationToken cancellationToken)
    {
        try
        {
            try
            {
                //var deviceinfo = (from D in _context.Set<UserDevices>()
                //                  where (D.DeviceSerial == request.DeviceSerial && D.CompanyId == CompanyID && D.UserId == UserID) ||
                //                      (D.DeviceModel == request.DeviceModel && D.DeviceType == request.DeviceType && D.CompanyId == CompanyID && D.UserId == UserID)
                //                  select D).FirstOrDefault();
                var dev = _context.Set<UserDevice>().ToList();
                var deviceinfo = _context.Set<UserDevice>().Where(d => d.DeviceModel == request.DeviceModel && d.UserId == UserID
                ).FirstOrDefault();
                if (deviceinfo != null)
                {
                    //if(!string.IsNullOrEmpty(IP.PushDeviceId.Trim())) {
                    deviceinfo.UserId = UserID;
                    deviceinfo.DeviceId = request.PushDeviceId;
                    deviceinfo.DeviceModel = request.DeviceModel;
                    deviceinfo.DeviceOs = request.DeviceOS;
                    deviceinfo.DeviceType = request.DeviceType;
                    deviceinfo.DeviceSerial = request.DeviceSerial;
                    deviceinfo.UpdatedOn = GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    deviceinfo.UpdatedBy = UserID;
                    deviceinfo.DeviceToken = Guid.NewGuid().ToString();
                    _context.SaveChangesAsync(cancellationToken);


                    UserLoginLog TblUserLog = new UserLoginLog();
                    TblUserLog.CompanyId = CompanyID;
                    TblUserLog.UserId = UserID;
                    TblUserLog.DeviceType = request.DeviceType;
                    TblUserLog.Ipaddress = Left(request.DeviceModel, 15);
                    TblUserLog.LoggedInTime = GetDateTimeOffset(DateTime.Now, TimeZoneId);

                    _context.AddAsync(TblUserLog, cancellationToken);

                    _context.SaveChangesAsync(cancellationToken);
                   
                    //}
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            string webpath = LookupWithKey("PORTAL");
            string SupportPhone = LookupWithKey("APP_SUPPORT_PHONE");
            string SupportEmail = LookupWithKey("APP_SUPPORT_EMAIL");
            string IncidentSirenPath = LookupWithKey("INCIDENT_SIREN_AUDIO_PATH");
            string PingSirenPath = LookupWithKey("PING_SIREN_AUDIO_PATH");
            string FBPage = LookupWithKey("CC_FB_PAGE");
            string LinkedInPage = LookupWithKey("CC_LINKEDIN_PAGE");
            string TwitterPage = LookupWithKey("CC_TWITTER_PAGE");
            string ForceUpdate = LookupWithKey("FORCE_UPDATE");
            bool DevAPIEnabled = false;
            bool.TryParse(LookupWithKey("API_DEV_MODE"), out DevAPIEnabled);
            string DevAPIURL = LookupWithKey("DEV_API_URL");
            string DevUsers = LookupWithKey("DEV_MODE_USERS");
            string MessageLength = LookupWithKey("MESSAGE_LENGTH");
            string AudioRecordMaxDuration = LookupWithKey("AUDIO_MAX_RECORD_DURATION");

            

            string ig = LookupWithKey("INITIALS_GENERATOR_URL");

            string[] roles = CCRoles();
            string AppVersion = GetAppVersion(request.DeviceType);
            string TrackingInterval = GetCompanyParameter("USER_TRACKING_INTERVAL", CompanyID);



            var thisuser = (from users in _context.Set<User>()
                            join userdevice in _context.Set<UserDevice>() on users.UserId equals userdevice.UserId
                            join company in _context.Set<Company>() on users.CompanyId equals company.CompanyId
                            where users.UserId == UserID && userdevice.DeviceSerial == request.DeviceSerial
                            select new { users, userdevice, company }).ToList();

            string userrole = thisuser.FirstOrDefault().users.UserRole.ToString().ToUpper();
            userrole = userrole.Replace("SUPERADMIN", "ADMIN");
            bool twofactor = Convert.ToBoolean(GetCompanyParameter("FORCE_2_FACTOR_AUTH_" + userrole, CompanyID));

            var getuserlogin = (from user in thisuser
                                select new LoginInfoReturnModel
                                {
                                    CompanyId = user.users.CompanyId,
                                    UserId = user.users.UserId,
                                    CustomerId = user.company.CustomerId,
                                    Portal = webpath,
                                    First_Name = user.users.FirstName,
                                    Last_Name = user.users.LastName,
                                    Primary_Email = user.users.PrimaryEmail,
                                    CompanyName = user.company.CompanyName,
                                    UploadPath = webpath + "uploads/" + CompanyID.ToString(),
                                    CompanyMasterPlan = (user.company.PlanDrdoc == null || user.company.PlanDrdoc == "") ? "" : webpath + "uploads/" + CompanyID.ToString() + "/companyplan/" + user.company.PlanDrdoc,
                                    CompanyLoginLogo = (user.company.CompanyLogoPath == null || user.company.CompanyLogoPath == "") ? "" : webpath + "uploads/" + CompanyID.ToString() + "/companylogos/" + user.company.IOslogo,
                                    iOSLogo = (user.company.IOslogo == null || user.company.IOslogo == "") ? "" : webpath + "uploads/" + CompanyID.ToString() + "/companylogos/" + user.company.IOslogo,
                                    AndroidLogo = (user.company.AndroidLogo == null || user.company.AndroidLogo == "") ? "" : webpath + "uploads/" + CompanyID.ToString() + "/companylogos/" + user.company.AndroidLogo,
                                    WindowsLogo = (user.company.WindowsLogo == null || user.company.WindowsLogo == "") ? "" : webpath + "uploads/" + CompanyID.ToString() + "/companylogos/" + user.company.WindowsLogo,
                                    UserDeviceID = user.userdevice.UserDeviceId,
                                    Token = user.userdevice.DeviceToken,
                                    UniqueGuiID = user.users.UniqueGuiId,
                                    UserStatus = user.users.Status,
                                    DeviceStatus = user.userdevice.Status,
                                    SupportPhone = SupportPhone,
                                    SupportEmail = SupportEmail,
                                    IncidentSiren = IncidentSirenPath,
                                    PingSiren = PingSirenPath,
                                    OverrideSilent = user.userdevice.OverrideSilent,
                                    SirenOn = user.userdevice.SirenOn,
                                    SoundFile = user.userdevice.SoundFile,
                                    FBPage = FBPage,
                                    TwitterPage = TwitterPage,
                                    LinkedInPage = LinkedInPage,
                                    AppVersion = AppVersion,
                                    ForceUpdate = ForceUpdate,
                                    FirstLogin = user.users.FirstLogin,
                                    UserRole = user.users.UserRole,
                                    UserLanguage = user.users.UserLanguage,
                                    TrackingStartTime = user.users.TrackingStartTime,
                                    TrackingEndTime = user.users.TrackingEndTime,
                                    TrackingInterval = TrackingInterval,
                                    DevMode = DevAPIEnabled,
                                    DevAPI = DevAPIURL,
                                    MessageLength = MessageLength,
                                    ActiveOffDuty = user.users.ActiveOffDuty,
                                    AudioRecordMaxDuration = AudioRecordMaxDuration,
                                    UniqueKey = user.company.UniqueKey,
                                    TwoFactorLogin = twofactor,
                                    UserPhoto = string.IsNullOrEmpty(user.users.UserPhoto) ? "" : user.users.UserPhoto
                                }).FirstOrDefault();
            return getuserlogin;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}