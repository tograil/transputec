using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.App.Services
{
    public interface IAppService
    {
        Task<AppHomeReturn> AppHome(int companyID, int userId, int userDeviceID, string token);
        Task<string> SendFeedback(string deviceType, string deviceOS, string userEmail, string deviceModel, string feedbackMessage);
        Task<string> UpdateDevice(bool isSirenOn, bool overrideSilent, string soundFile, string updateType, string language, string deviceSerial, int companyID, int userId);
        Task<List<string>> CCPhoneNumbers();
        Task<string> ReferToFriend(string referToName, string referToEmail, string referMessage, string userEmail, int userID);
        Task<bool> CaptureUserLocation(List<LocationInfo> userLocations, int userId, int userDeviceID, int companyID, string timeZoneId);
        Task<List<LanguageItem>> GetLanguage(string locale);
        Task<List<UserLocation>> GetUserLocationsList(int userDeviceID, int pLength, string action = "list");
        Task<bool> UpdatePushToken(int userDeviceID, string pushDeviceId);
        Task<bool> UpdateTrackMe(bool enabled, string trackType, int activeIncidentID, int userId, int userDeviceID, int companyID, string timeZoneId, string latitude, string longitude);
        Task AddTrackMe(int userID, string trackType, int userDeviceID, int activeIncidentID, int companyID, string timeZoneId);
        Task<List<AppLanguageList>> GetAppLanguage();
    }
}
