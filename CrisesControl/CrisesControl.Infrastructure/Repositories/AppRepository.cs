using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.App;
using CrisesControl.Core.App.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly CrisesControlContext _context;
        private readonly DBCommon DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppRepository(CrisesControlContext context, DBCommon DBC, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this.DBC = DBC;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<AppHomeReturn> AppHome(int companyID,int userId, int userDeviceID,string token)
        {
            
            try
            {
                string DeviceType = string.Empty;

                int TotalPingMessageForUser = 1, TotalPingMessageForUserWithInKPI = 0, TotalPingMessageUnACK = 0, TotaIncidentMessageForUser = 1;
                int TotaIncidentMessageForUserWithInKPI = 0, TotalIncidntMessageUnACK = 0, TaskCount = 0;

                GetIncidentPingRating(companyID, userId, out TotalPingMessageForUser, out TotalPingMessageForUserWithInKPI, out TotalPingMessageUnACK,
                    out TotaIncidentMessageForUser, out TotaIncidentMessageForUserWithInKPI, out TotalIncidntMessageUnACK, out TaskCount);
                //PING                    
                double devidePingVal = ((TotalPingMessageForUserWithInKPI * 100) / TotalPingMessageForUser);
                int PingStar = 1;

                if (devidePingVal <= 20)
                {
                    PingStar = 1;
                }
                else if (devidePingVal <= 40)
                {
                    PingStar = 2;
                }
                else if (devidePingVal <= 60)
                {
                    PingStar = 3;
                }
                else if (devidePingVal <= 80)
                {
                    PingStar = 4;
                }
                else
                {
                    PingStar = 5;
                }

                //Incident
                double devideIncidentVal = ((TotaIncidentMessageForUserWithInKPI * 100) / TotaIncidentMessageForUser);
                int IncidedntStar = 1;

                if (devideIncidentVal <= 20)
                {
                    IncidedntStar = 1;
                }
                else if (devideIncidentVal <= 40)
                {
                    IncidedntStar = 2;
                }
                else if (devideIncidentVal <= 60)
                {
                    IncidedntStar = 3;
                }
                else if (devideIncidentVal <= 80)
                {
                    IncidedntStar = 4;
                }
                else
                {
                    IncidedntStar = 5;
                }

                string AppHomeTextTitleVal = DBC.LookupWithKey("APPHOMETEXT");
                string AppHomeTextDescriptionVal = DBC.LookupWithKey("APPHOMETEXT", "");
                string copyrightinfo = DBC.LookupWithKey("COPYRIGHT_INFO");
                string TutorialPathVal = DBC.LookupWithKey("TUTORIALPATH");
                string TermsAndConditionsUrlVal = DBC.LookupWithKey("TERMSANDCONDITIONSURL");
                string PrivacyPolicyUrlVal = DBC.LookupWithKey("PRIVACYPOLICYURL");
                string WhatsNewUrlVal = DBC.LookupWithKey("WHATSNEWURL");
                string Portal = DBC.LookupWithKey("PORTAL");
                string InviteUrl = DBC.LookupWithKey("INVITE_URL");
                string webpath = DBC.LookupWithKey("PORTAL");
                int ScrollThreashold = Convert.ToInt16(DBC.LookupWithKey("PING_LIST_THRESHOLD"));
                int PingListLimit = Convert.ToInt16(DBC.GetCompanyParameter("PING_LIST_LIMIT", companyID));

                bool HasTask = false;

                if (TaskCount > 0)
                    HasTask = true;

                var TrackingModes = await _context.Set<TrackMe>()
                                     .Where(TM=> TM.UserDeviceId == userDeviceID &&
                                     TM.TrackMeStopped.Value.Year < 2000
                                     ).ToListAsync();

                var LangLastUpdated = await _context.Set<LanguageItem>().OrderByDescending(L=>L.LastUpdatedOn).Select(L=> L.LastUpdatedOn).FirstOrDefaultAsync();
                var company_logo = await _context.Set<Company>().Where(C=> C.CompanyId == companyID).Select(C=> C.ContactLogoPath).FirstOrDefaultAsync();

                AppHomeReturn getHomePageStat = new AppHomeReturn()
                {
                    PingCount = TotalPingMessageUnACK,
                    IncidentCount = TotalIncidntMessageUnACK,
                    PingStat = PingStar,
                    CompanyMessageLastUpdate = await DBC.LookupLastUpdate("APPHOMETEXT"),
                    CCMessageLastUpdate = await DBC.GetCompanyParameterLastUpdate("APP_HOME_TEXT", companyID),
                    IncidentStat = IncidedntStar,
                    ApiVersion = DBC.LookupWithKey("CURRENT_API_VERSION"),
                    CompanyText = DBC.GetCompanyParameter("APP_HOME_TEXT", companyID),
                    UsefulText = new UsefulText() { Title = AppHomeTextTitleVal, Description = AppHomeTextDescriptionVal },
                    CopyRightInfo = copyrightinfo,
                    TutorialPath = TutorialPathVal,
                    TermsAndConditionsUrl = TermsAndConditionsUrlVal,
                    PrivacyPolicyUrl = PrivacyPolicyUrlVal,
                    WhatsNewUrl = WhatsNewUrlVal,
                    MyTaskURL = Portal + "tools/mytask_app/" + token,
                    HasTask = HasTask,
                    InviteUrl = InviteUrl,
                    PhoneContactLogo = (company_logo == null || company_logo == "") ? "" : webpath + "uploads/" + companyID.ToString() + "/companylogos/" + company_logo,
                    LangLastUpdated = (DateTimeOffset)LangLastUpdated,
                    PhoneVerifier = "+447700900500",
                    PingListThreshold = ScrollThreashold,
                    PingListLimit = PingListLimit,
                    CompanyParams = await _context.Set<CompanyParameter>()
                                     .Where(CP=> CP.CompanyId == companyID)
                                     .Select(CP=> new CompanyParam { Name = CP.Name, Value = CP.Value }).ToListAsync(),
                    MessageMethods = await _context.Set<CompanyComm>().Include(CM=>CM.CommsMethod)                                      
                                      .Where(CC=> CC.CompanyId == companyID && CC.ServiceStatus == true && CC.Status == 1)
                                      .Select(CC=> new CommsMethods
                                      {
                                          MethodId = CC.MethodId,
                                          MethodName = CC.CommsMethod.MethodName,
                                          ServiceStatus = CC.ServiceStatus,
                                          Status = CC.Status
                                      }).ToListAsync(),
                    TrackMeTravel = TrackingModes.Where(w => w.TrackType == "TRAVEL").Any(),
                    TrackMeIncident = TrackingModes.Where(w => w.TrackType == "INCIDENT").Any(),
                    AppIconURL =  await _context.Set<AppLanguage>()
                                  .Where(L=> L.Status == 1)
                                  .Select(L=> new AppIconURL
                                  {
                                      IconURL = L.IconUrl,
                                      LangCode = L.Locale,
                                      Platform = L.Platform,
                                      LastUpdate = (DateTimeOffset)L.LastUpdatedDate
                                  }).ToListAsync()
                };

                if (getHomePageStat != null)
                {

                    return getHomePageStat;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GetIncidentPingRating(int companyId, int userId, out int totalPingSentForUser, out int totalPingInKPIForUser, out int totalPingUnACKForUser,
    out int totalIncidentSentForUser, out int totalIncidentInKPIForUser, out int totalIncidentUnACKForUser, out int taskCount)
        {

            totalPingSentForUser = 1;
            totalPingInKPIForUser = 0;
            totalPingUnACKForUser = 0;
            
            totalIncidentSentForUser = 1;
            totalIncidentInKPIForUser = 0;
            totalIncidentUnACKForUser = 0;

            taskCount = 0;

            try
            {

                var pUserID = new SqlParameter("@UserID", userId);
                var pCompanyId = new SqlParameter("@CompanyID", companyId);

                var IncidentPingStarVal = _context.Set<AppHomeRatings>().FromSqlRaw("exec Pro_Get_Incident_Ping_Rating @UserID, @CompanyID",
                    pUserID, pCompanyId).FirstOrDefault();

                if (IncidentPingStarVal != null)
                {
                    totalPingSentForUser = Convert.ToInt32(IncidentPingStarVal.TotalPingSent > 0 ? IncidentPingStarVal.TotalPingSent : 1);
                    totalPingInKPIForUser = Convert.ToInt32(IncidentPingStarVal.TotalPingInKPI);
                    totalPingUnACKForUser = Convert.ToInt32(IncidentPingStarVal.TotalPingSent - IncidentPingStarVal.TotalPingACK);
                    
                    totalIncidentSentForUser = Convert.ToInt32(IncidentPingStarVal.TotalIncidentSent > 0 ? IncidentPingStarVal.TotalIncidentSent : 1);
                    totalIncidentInKPIForUser = Convert.ToInt32(IncidentPingStarVal.TotalIncidentInKPI);
                    totalIncidentUnACKForUser = Convert.ToInt32(IncidentPingStarVal.TotalIncidentUnAck);
                    
                    taskCount = IncidentPingStarVal.PendingTask;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> SendFeedback(string deviceType, string deviceOS,string userEmail,string deviceModel, string feedbackMessage)
        {
       
            try
            {
                string hostname = string.Empty;
                string fromadd = string.Empty;
                string feedbackaddress = string.Empty;
                string Message = string.Empty;

                hostname = DBC.LookupWithKey("SMTPHOST");

                fromadd = DBC.LookupWithKey("EMAILFROM");
                feedbackaddress = DBC.LookupWithKey("SENDFEEDBACKTO");
                  
                if ((!string.IsNullOrEmpty(hostname)) && (!string.IsNullOrEmpty(fromadd)))
                {
                    string messagebody = "You have received a new feedback from the app by (" + userEmail + "), details are as below<br>";
                    messagebody = messagebody + "<b>Message:</b><br>";
                    messagebody = messagebody + feedbackMessage + "<br>";
                    messagebody = messagebody + "<b>Device Information:</b> <br>";
                    messagebody = messagebody + "<b>Device Type:</b> " + deviceType;
                    messagebody = messagebody + "<br><b>Device OS:</b> " + deviceOS;
                    messagebody = messagebody + "<br><b>Device Model:</b> " + deviceModel;

                    SendEmail sendEmail = new SendEmail(_context,DBC);
                    string[] AdminEmail = feedbackaddress.Split(',');
                    bool ismailsend = sendEmail.Email(AdminEmail, messagebody, fromadd, hostname, "Feedback from App");
                    if (ismailsend == false)
                    {
                        Message = "Email sending failed! Please try again.";
                    }
                    else
                    {
                       Message = "We have received your feedback, we'll look into it shortly";
                    }
                }
                else
                {
                    Message = "SMTP is not configured properly";
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> ReferToFriend(string referToName,string referToEmail,string referMessage,string userEmail, int userID)
        {
           
            try
            {
                string Message = string.Empty;
                var UserInfo = await _context.Set<User>()
                                .Where(U=> U.UserId == userID)
                                .Select(U=> new
                                {
                                    Username = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                                }).FirstOrDefaultAsync();
                if (UserInfo != null)
                {
                    var message = _context.Set<SysParameter>().Single(L => L.Name == "REFERTOFRIEND");
                    var hostname = string.Empty;
                    var fromadd = string.Empty;
                    hostname = DBC.LookupWithKey("SMTPHOST");
                    fromadd = DBC.LookupWithKey("EMAILFROM");

                    if ((!string.IsNullOrEmpty(hostname)) && (!string.IsNullOrEmpty(fromadd)))
                    {
                        string messagebody = message.Value;
                        messagebody = messagebody.Replace("{REFER_TO_NAME}", referToName);
                        messagebody = messagebody.Replace("{REFER_TO_EMAIL}", referToEmail);
                        messagebody = messagebody.Replace("{REFER_TO_MESSAGE}", referMessage);
                        messagebody = messagebody.Replace("{REFER_NAME}", DBC.UserName(UserInfo.Username));
                        messagebody = messagebody.Replace("{REFER_EMAIL}", userEmail);

                        string Subject = message.Description;

                        string ToAddress = referToEmail;

                        SendEmail sendEmail = new SendEmail(_context,DBC);

                        string[] toEmails = { ToAddress };

                        bool ismailsend = sendEmail.Email(toEmails, messagebody, fromadd, hostname, Subject);

                        if (ismailsend == false)
                        {
                           Message = "Email sending failed! Please try again.";
                        }
                        else
                        {
                           Message = "Email has been sent successfully";
                        }
                    }
                    else
                    {
                        Message = "SMTP is not configured properly";
                    }
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> UpdateDevice(bool isSirenOn, bool overrideSilent,string soundFile,string updateType, string language,string deviceSerial, int companyID, int userId)
        {
            
            try
            {
                string message = string.Empty;
                var UserDev = await  _context.Set<UserDevice>()
                               .Where(UD=>
                                 UD.DeviceSerial == deviceSerial
                                 && UD.CompanyId == companyID).FirstOrDefaultAsync();

                if (UserDev != null)
                {

                    if (updateType == UpdateType.SIRENON.ToUString())
                    {
                        UserDev.SirenOn = isSirenOn;
                        _context.Update(UserDev);
                        message = "Device updated.";
                    }
                    else if (updateType == UpdateType.DNDON.ToUString())
                    {
                        UserDev.OverrideSilent = overrideSilent;
                        _context.Update(UserDev);
                        message = "Device updated.";
                    }
                    else if (updateType == UpdateType.SOUND.ToUString())
                    {
                        UserDev.SoundFile = soundFile;
                        _context.Update(UserDev);
                        message = "Siren sound updated";
                    }
                    else if (updateType == UpdateType.LANGUAGE.ToUString())
                    {
                        var user = await _context.Set<User>().Where(U=> U.UserId == userId).FirstOrDefaultAsync();
                        user.UserLanguage = language;
                        _context.Update(user);
                        message = "Language updated";
                    }

                   await _context.SaveChangesAsync();
                   return message;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> CCPhoneNumbers()
        {
            var numbers = await  _context.Set<PhoneNumberMapping>().Select(N=> N.FromNumber).Distinct().ToListAsync();            
            return numbers;
        }
        public async Task<bool> CaptureUserLocation(List<LocationInfo> userLocations, int userId, int userDeviceID, int companyID,string timeZoneId)
        {
            try
            {
                bool CollectAllLog = false;
                bool.TryParse(DBC.LookupWithKey("COLLECT_ALL_COORD_REQUEST"), out CollectAllLog);

                if (userLocations != null)
                {
                    var get_last_loc = await _context.Set<UserLocation>()
                                        .Where(UL=> UL.UserId == userId &&
                                        UL.UserDeviceId == userDeviceID).OrderByDescending(o => o.LocationId).FirstOrDefaultAsync();

                    if (get_last_loc != null)
                    {
                        Messaging MSG = new Messaging(_context,_httpContextAccessor);
                        double LastLat =Convert.ToDouble( get_last_loc.Lat);
                        double LastLng = Convert.ToDouble(get_last_loc.Long);
                        foreach (LocationInfo loc in userLocations)
                        {
                            if (!string.IsNullOrWhiteSpace(loc.Latitude) && !string.IsNullOrWhiteSpace(loc.Longitude))
                            {
                                double locLat = 0;
                                double loclng = 0;

                                double.TryParse(loc.Latitude, out locLat);
                                double.TryParse(loc.Longitude, out loclng);

                                if ((LastLat != locLat || LastLng != loclng) | CollectAllLog == true)
                                {
                                    string loc_address = DBC.RetrieveFormatedAddress(loc.Latitude, loc.Longitude);
                                   MSG.AddUserLocation(userId, userDeviceID, locLat, loclng, loc_address, loc.UserDeviceTime, timeZoneId, companyID);
                                    LastLat = locLat;
                                    LastLng = loclng;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        Messaging MSG = new Messaging(_context,_httpContextAccessor);
                        double LastLat = 0;
                        double LastLng = 0;

                        foreach (LocationInfo loc in userLocations)
                        {
                            double locLat = 0;
                            double loclng = 0;

                            loc.Latitude = !string.IsNullOrEmpty(loc.Latitude) ? loc.Latitude.Replace(",", ".") : "0";
                            loc.Longitude = !string.IsNullOrEmpty(loc.Longitude) ? loc.Longitude.Replace(",", ".") : "0";

                            double.TryParse(loc.Latitude, out locLat);
                            double.TryParse(loc.Longitude, out loclng);

                            if ((LastLat != Convert.ToDouble(loc.Latitude) || LastLng != Convert.ToDouble(loc.Longitude)) || CollectAllLog == true)
                            {
                                if (!string.IsNullOrWhiteSpace(loc.Latitude) && !string.IsNullOrWhiteSpace(loc.Longitude))
                                {
                                    string loc_address = DBC.RetrieveFormatedAddress(loc.Latitude, loc.Longitude);

                                   MSG.AddUserLocation(userId, userDeviceID, Convert.ToDouble(loc.Latitude), Convert.ToDouble(loc.Longitude), loc_address, loc.UserDeviceTime, timeZoneId, companyID);
                                    LastLat = Convert.ToDouble(loc.Latitude);
                                    LastLng = Convert.ToDouble(loc.Longitude);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<UserLocation>> GetUserLocationsList(int userDeviceID, int pLength, string action = "list")
        {
            if (action == "clear")
            {
                var remove = await _context.Set<UserLocation>().Where(UL=> UL.UserDeviceId == userDeviceID).ToListAsync();
                _context.RemoveRange(remove);
                await _context.SaveChangesAsync();
            }
            return await  _context.Set<UserLocation>()
                    .Where(UL=> UL.UserDeviceId == userDeviceID).OrderByDescending(UL=>UL.LocationId)
                   .Take(pLength).ToListAsync();
        }

        public async Task<List<LanguageItem>> GetLanguage(string locale)
        {
            locale = (string.IsNullOrEmpty(locale) ? "en" : locale);
            if (locale.ToUpper() == "ALL")
            {
                var langlist =  await _context.Set<LanguageItem>().ToListAsync();
                return langlist;
            }
            else
            {
                var langlist = await _context.Set<LanguageItem>().Where(LI=> LI.Locale == locale).ToListAsync();
                return langlist;
            }
        }
        public async Task<bool> UpdateTrackMe(bool enabled, string trackType, int activeIncidentID, int userId, int userDeviceID, int companyID, string timeZoneId, string latitude, string longitude)
        {
            try
            {
                bool Rtn = false;
                if (enabled)
                {
                   await  AddTrackMe(userId, trackType, userDeviceID, activeIncidentID, companyID, timeZoneId);
                    Rtn = true;
                }
                else
                {
                    var devc = await  _context.Set<TrackMe>()
                                .Where(TM=> TM.UserDeviceId == userDeviceID && TM.TrackType == trackType && TM.TrackMeStopped.Value.Year < 2000
                               ).ToListAsync();
                    foreach (var dev in devc)
                    {
                        dev.TrackMeStopped = DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                        _context.Update(dev);
                    }
                   await _context.SaveChangesAsync();
                    Rtn = false;
                }

                //Add First User location;
              
                List<LocationInfo> LI = new List<LocationInfo>();
                LI.Add(new LocationInfo { Latitude = latitude, Longitude = longitude, UserDeviceTime = DateTime.Now });
               await CaptureUserLocation(LI,userId,userDeviceID,companyID,timeZoneId);
                return Rtn;

            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task AddTrackMe(int userID, string trackType, int userDeviceID, int activeIncidentID, int companyID, string timeZoneId)
        {
            TrackMe TM = new TrackMe();
            TM.UserDeviceId = userDeviceID;
            TM.UserId = userID;
            TM.CompanyId = companyID;
            TM.TrackType = trackType;
            TM.TrackMeStarted = DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
            TM.TrackMeStopped = DBC.DbDate();
            TM.ActiveIncidentId = activeIncidentID;
            await _context.AddAsync(TM);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AppLanguageList>> GetAppLanguage()
        {
            try
            {
                var langs = await  _context.Set<AppLanguage>()
                             .Where(L=> L.Status == 1)
                             .Select(L=> new AppLanguageList {
                                 LanguageName= L.LanguageName, 
                                 FlagIcon=L.FlagIcon,
                                 Locale= L.Locale }).Distinct().ToListAsync();
                return langs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdatePushToken(int userDeviceID, string pushDeviceId)
        {
            try
            {
                var device = await  _context.Set<UserDevice>().Where(D=> D.UserDeviceId == userDeviceID).FirstOrDefaultAsync();
                if (device != null)
                {
                    device.DeviceId = pushDeviceId;
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
