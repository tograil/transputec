using CrisesControl.Core.Import;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.DBCommon.Repositories
{
    public interface IDBCommonRepository
    {
        Task<StringBuilder> ReadHtmlFile(string fileCode, string source, int companyId, string subject, string provider = "AWSSES");
        Task AddTrackingDevice(int companyID, int userDeviceID, string deviceAddress, string deviceType, int messageListID = 0);
        Task<string> LookupWithKey(string key, string Default = "");
        Task<string> UserName(UserFullName strUserName);
        Task<int> IncidentNote(int objectId, string noteType, string notes, int companyId, int userId);
        Task<DateTime> GetLocalTime(string timeZoneId, DateTime? paramTime = null);
        Task<DateTimeOffset> GetDateTimeOffset(DateTime crTime, string timeZoneId = "GMT Standard Time");
        Task<string> ToCurrency(decimal amount, int points = 2);
        Task<string> PWDencrypt(string strPwdString);
        Task<string> getapiversion();
        Task<string[]> CCRoles(bool addKeyHolder = false, bool addUser = false);
        Task<string> GetCompanyParameter(string key, int companyId, string Default = "", string customerId = "");
        Task CreateObjectRelationship(int targetObjectId, int sourceObjectId, string relationName, int companyId, int createdUpdatedBy, string timeZoneId, string relatinFilter = "");
          Task CreateNewObjectRelation(int sourceObjectId, int targetObjectId, int objMapId, int createdUpdatedBy, string timeZoneId, int companyId);
        Task<int> AddPwdChangeHistory(int userId, string newPassword);
        Task<DateTimeOffset> ConvertToLocalTime(string timezoneId, DateTimeOffset paramTime);
        Task DeleteScheduledJob(string jobName, string group);
        Task<string> GetTimeZoneVal(int userId);
        string Left(string str, int lngth, int stpoint = 0);
        Task<string> FixMobileZero(string strNumber);
        Task<string> GetTimeZoneByCompany(int companyId);
        Task CreateLog(string level, string message, Exception ex = null, string controller = "", string method = "", int companyId = 0);
        Task UpdateLog(string strErrorID, string strErrorMessage, string strControllerName, string strMethodName, int intCompanyId);
        Task GetSetCompanyComms(int companyId);
        Task _set_comms_status(int companyId, List<string> methods, bool status);
        Task<string> GetPackageItem(string itemCode, int companyId);
        Task<DateTimeOffset> GetNextReviewDate(DateTimeOffset currentDateTime, string frequency);
        Task<DateTimeOffset> GetNextReviewDate(DateTimeOffset currentReviewDate, int companyID, int reminderCount, int reminderCounter);
        Task<bool> verifyLength(string str, int minLength, int maxLength);
        Task<bool> IsPropertyExist(dynamic settings, string name);
        Task<LatLng> GetCoordinates(string address);
        Task<bool> connectUNCPath(string uncPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "");
        Task<string> PureAscii(string str, bool keepAccent = false);
        Task<string> RandomPassword(int length = 8, int complexity = 4);
        Task RemoveUserObjectRelation(string relationName, int userId, int sourceObjectId, int companyId, int currentUserId, string timeZoneId);
        Task RemoveUserDevice(int userId, bool tokenReset = false);
        Task MessageProcessLog(int messageId, string eventName, string methodName = "", string queueName = "", string additionalInfo = "");
        Task<string> Getconfig(string key, string defaultVal = "");
        Task<string> FormatMobile(string ISD, string mobile);
        Task<bool> IsTrue(string boolVal, bool Default = true);
        Task<int> ChunkString(string str, int chunkSize);
        Task<dynamic> InitComms(string API_CLASS, string apiClass = "", string clientId = "", string clientSecret = "");
        Task<List<NotificationUserList>> GetUniqueUsers(List<NotificationUserList> list1, List<NotificationUserList> list2, bool participantCheck = true);
        Task<List<AckOption>> GetAckOptions(int messageId);
        Task<string> GetValueByIndex(List<string> valueList, int indexVal);
        Task LocalException(string error, string message, string controller = "", string method = "", int companyId = 0);
        Task<bool> IsDayLightOn(DateTime thisDate);
        Task CancelJobsByGroup(string jobGroup);
        Task DeleteOldFiles(string dirName);
        Task CreateSOPReviewReminder(int incidentId, int sopHeaderId, int companyId, DateTimeOffset nextReviewDate, string reviewFrequency, int reminderCount);
        Task<List<SocialHandles>> GetSocialServiceProviders();
        Task<int> SegregationWarning(int companyId, int userID, int incidentId);
        Task<List<SocialIntegraion>> GetSocialIntegration(int companyId, string accountType);
        Task SaveParameter(int parameterID, string parameterName, string parameterValue, int currentUserID, int companyID, string timeZoneId);
        Task<int> AddCompanyParameter(string name, string value, int companyId, int currentUserId, string timeZoneId);
        Task<bool> OnTrialStatus(string companyProfile, bool currentTrial);
        void GetStartEndDate(bool isThisWeek, bool isThisMonth, bool isLastMonth, ref DateTime stDate, ref DateTime enDate, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Return> Return(int errorId = 100, string errorCode = "E100", bool status = false, string message = "FAILURE", object data = null, int resultId = 0);
        Task<string> PhoneNumber(PhoneNumber strPhoneNumber);
        Task<DateTimeOffset> LookupLastUpdate(string Key);
        Task<DateTimeOffset> GetCompanyParameterLastUpdate(string Key, int CompanyId);
        Task<DateTime> DbDate();
        Task UpdateUserLocation(int userid, string latitude, string longitude, string timeZoneId);
        Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",");
        Task ModelInputLog(string controllerName, string methodName, int userID, int companyID, dynamic data);
        Task<DateTimeOffset> ToNullIfTooEarlyForDb(DateTimeOffset date, bool convertUTC = false);
        Task<bool> AddUserTrackingDevices(int userID, int messageListID = 0);
        Task UpdateUserDepartment(int userId, int departmentId, int createdUpdatedBy, int companyId, string timeZoneId);
        Task<string> RetrieveFormatedAddress(string lat, string lng);





    }
}
