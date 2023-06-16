
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Base;
using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Communication.Services
{
    public interface ICommsLogService
    {
        Task GetCommsLogs();
        Task GetCommsLogsForced();
        Task DumpCallLogs();
        Task ProcessCallLogs(CallResource item, string session);
        Task DumpSMSLogs();
        Task ProcessCMSMSLog(CMResult item, string session);
        Task<CMResult> GetCMSMS(string refId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task DownloadAndCreateCMSMSLog(string cloudMessageId, DateTimeOffset createdOn);
        Task DownloadAndCreateTwilioLog(string cloudMessageId, string method);
        Task<CMSMSResponse> CMSMSLog(DateTime startDate, DateTime endDate, string Direction);
        Task ProcessSMSLog(MessageResource item, string session);
        Task DumpRecordings();
        Task ProcessRecLog(RecordingResource item, string session);
        Task<bool> SendLogDumpToApi(string logType, List<CallResource> calls, List<MessageResource> texts, List<RecordingResource> recs);
        Task<bool> CreateCommsQueueSession(string sessionId);
        Task<bool> ProcessCommsLogs(string sessionId);
        Task CreateCommsLogDump(string session, string sid, string commType, string status, string from, string to, string direction, decimal price, string answeredBy,
         string priceUnit, int numSegments, string body, int duration, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset startTime,
         DateTimeOffset endTime, string errorCode = "", string errorMessage = "", int logStatus = 0, string commsProvider = "TWILIO");
        Task<TwilioBatch> GetTwilioBatchTime(string logType, string commsProvider);
        Task<MessageResource> GetTwilioMessage(string sid);
        Task<CallResource> GetTwilioCall(string sid);
        Task<ConferenceResource> GetTwilioConf(string sid);
        Task<MessageResource> GetTwilioMessageByApi(string sid);
        Task<CallResource> GetTwilioCallByApi(string sid);
        Task<ConferenceResource> GetTwilioConfByApi(string sid);
        Task<ResourceSet<RecordingResource>> GetTwilioRecByApi(string sid);
        Task GetConferenceLog();
        Task GetCallRecordingLog(CallResource call);
        Task GetCommsPrice();
        Task DownloadTwilioPricing();
        Task GetVoicePricing(string iso2Code, string countryCode);
        Task GetSMSPricing(string iso2Code, string countryCode, string dialingCode);
        Task<int> AddTwilioPrice(string iso2Code, string iso3Code, string channelType, string prefix, decimal basePrice, decimal currentPrice, string friendyName, string numberType = "");
        Task GetTwilioVoiceCountries();
        Task GetTwilioSMSCountries();
        Task UpdateBalance_SMS(string smsSid);
        Task UpdateBalance_PHONE(string callSid);
        Task<TwilioPriceByNumber> GetLocalPrice(string phoneNumber, string channel);
        Task ClearTwilioLogs(int companyID);
        Task<List<TwilioLogToClear>> GetTwilioLogsToClear(int CompanyID);
        Task CreateCommsLog(string sid, string commType, string status, string from, string to, string direction, decimal price, string answeredBy, string priceUnit,
            int numSegments, string body, int duration, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset startTime, DateTimeOffset endTime,
            string errorCode = "", string errorMessage = "", string commsProvider = "");
    }
}
