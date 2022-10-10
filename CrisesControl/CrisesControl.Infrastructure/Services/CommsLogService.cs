using CrisesControl.Core.Communication;
using CrisesControl.Core.Models;
using CrisesControl.Core.System;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Base;
using static CrisesControl.Infrastructure.Repositories.DBCommonRepository;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Communication.Services;
using CrisesControl.Core.Queues.Services;

namespace CrisesControl.Infrastructure.Services
{
    public class CommsLogService: ICommsLogService
    {
        private readonly CrisesControlContext db;
        private readonly IDBCommonRepository DBC;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string PHONESID = string.Empty;
        private string PHONETOKEN = string.Empty;
        //private double COMMS_LOG_AGE_HOURS = 2;
        //private string RECORDING_URL = string.Empty;
        private int RetryCount = 0;
        private string TwilioRoutingApi = string.Empty;
        private bool SendInDirect = false;
        public event UpdateHandler EntityUpdate;
        private string CM_CLIENTID = string.Empty;
        private readonly IQueueMessageService _queueHelper;
        private readonly IMessageService _MSG;
        public CommsLogService(CrisesControlContext db, IHttpContextAccessor httpContextAccessor, IDBCommonRepository _DBC, IMessageService MSG, IQueueMessageService queue)
        {
            this.db = db;
            this._httpContextAccessor = httpContextAccessor;
            this.DBC = _DBC;
            this._MSG = MSG;
            _queueHelper = queue;
        }

        public CommsLogService()
        {
            PHONESID = DBC.LookupWithKey("PHONESID").ToString();
            PHONETOKEN = DBC.LookupWithKey("PHONETOKEN").ToString();
            CM_CLIENTID = DBC.LookupWithKey("CM_CLIENTID").ToString();
            //bool log_age = Double.TryParse(DBC.LookupWithKey("COMMS_LOG_AGE_HOURS"), out COMMS_LOG_AGE_HOURS);
            //RECORDING_URL = DBC.LookupWithKey("RECORDING_URL");

            TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API").ToString();

            int.TryParse(DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT").ToString(), out RetryCount);
            string sendDirect = DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION").ToString();
            string indirect = DBC.IsTrue(sendDirect, false).ToString();
            SendInDirect = Convert.ToBoolean(indirect);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }
        public void FireUpdateEvent(UpdateEventArgs e) {
            EntityUpdate?.Invoke(this, e);
        }

        public async Task GetCommsLogs()
        {
            //Create function to get twilio log batch interval
            if (!SendInDirect) {
                //DBC.CreateLog("INFO", "Running the functions just now");
               await DumpCallLogs();
               await DumpSMSLogs();
              await  DumpRecordings();
            }
            //GetTwilioSMSLog();
            //GetTwilioCallLog();
           await  GetConferenceLog();
            //GetTwoFactorAuthLog();
        }

        public async Task GetCommsLogsForced()
        {
           await  ForceLogCollection();
        }

        public async Task ForceLogCollection()
        {
            try
            {
                string session = Guid.NewGuid().ToString();

                TwilioClient.Init(PHONESID, PHONETOKEN);

                DateTimeOffset checkTime = await DBC.GetDateTimeOffset(DateTime.Now.AddHours(-2));

                var getSMSList =await (from MT in db.Set<MessageTransaction>()
                                  where MT.CloudMessageId != null && (MT.MethodName == "TEXT" || MT.MethodName == "PHONE") &&
                                  MT.LogCollected == false
                                  select MT).ToListAsync();

                foreach (var msg in getSMSList) {
                    try {
                        if (!string.IsNullOrEmpty(msg.CloudMessageId)) {

                            //if(DateTimeOffset.Now.Subtract((DateTimeOffset)msg.CreatedOn).TotalHours > 2 && msg.CloudMessageId.Length > 30) {

                            if (msg.CommsProvider == "TWILIO")
                            {
                                if (msg.MethodName.ToUpper() == "TEXT")
                                {
                                    var message = await  GetTwilioMessage(msg.CloudMessageId);
                                    if (message != null)
                                    {
                                       await ProcessSMSLog(message, session);
                                    }
                                }
                                else if (msg.MethodName.ToUpper() == "PHONE")
                                {
                                    var call = await  GetTwilioCall(msg.CloudMessageId);
                                    if (call != null)
                                    {
                                      await  ProcessCallLogs(call, session);
                                    }
                                }
                            }
                            else if (msg.CommsProvider == "CM")
                            {
                                var sms = await GetCMSMS(msg.CloudMessageId, msg.CreatedOn.Value.AddSeconds(-10), msg.CreatedOn.Value.AddSeconds(10));
                                if (sms != null)
                                {
                                   await ProcessCMSMSLog(sms, session);
                                }
                            }
                            //}
                        }

                    } catch (Exception ex) {
                        throw ex;
                    }
                }

                var result = await CreateCommsQueueSession(session);

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task DumpCallLogs()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = await GetTwilioBatchTime("PHONE", "TWILIO");
                int count = 1;

                //DBC.CreateLog("INFO", "Running the Call Logs data collecton");

                if (twlbatch.PendingLogs > 0) {
                    var items = CallResource.Read(endTimeAfter: twlbatch.StartTime.ToUniversalTime().DateTime, endTimeBefore: twlbatch.EndTime.ToUniversalTime().DateTime);

                    string session = Guid.NewGuid().ToString();

                    //DBC.CreateLog("INFO", "StarTime: " + twlbatch.StartTime.ToUniversalTime().DateTime + " | EndTime: " + twlbatch.EndTime.ToUniversalTime().DateTime, null, "", "DumpCallLogs");

                    List<CallResource> LogsPush = new List<CallResource>();

                    foreach (var item in items) {
                        LogsPush.Add(item);
                        Console.WriteLine("Message count" + count + " => " + item.Sid);
                       await ProcessCallLogs(item, session);
                        count++;
                    }
                    //todo create a memory object of items and push it to MEA like recipeints.
                    //DBC.CreateLog("INFO", "Total Items: " + LogsPush.Count);

                  await  SendLogDumpToApi("PHONE", LogsPush, null, null);

                   await CreateCommsQueueSession(session);
                }
                //Console.ReadLine();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task ProcessCallLogs(CallResource item, string session)
        {
            try
            {
                decimal Price = item.Price == null ? 0 : Convert.ToDecimal(item.Price);
                int Duration = item.Duration == null ? 0 : Convert.ToInt32(item.Duration);
                string answered_by = item.AnsweredBy == null ? "self" : item.AnsweredBy;

                DateTime StartTime = item.StartTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)item.StartTime;
                DateTime EndTime = item.EndTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)item.EndTime;

               await CreateCommsLogDump(session, item.Sid, "PHONE", item.Status.ToString(), item.From, item.To, item.Direction, Price, answered_by, "USD", 0, "", Duration,
                        (DateTime)item.DateCreated, (DateTime)item.DateUpdated, StartTime, EndTime, commsProvider: "TWILIO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DumpSMSLogs()
        {
            try
            {

                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = await  GetTwilioBatchTime("TEXT", "TWILIO");
                TwilioBatch CMbatch =await  GetTwilioBatchTime("TEXT", "CM");

                string session = Guid.NewGuid().ToString();

                if (twlbatch.PendingLogs > 0) {

                    //CM Text log outgoing
                    CMSMSResponse cmitems_out =await CMSMSLog(CMbatch.StartTime.UtcDateTime, CMbatch.EndTime.UtcDateTime, "out");
                    if (cmitems_out != null)
                    {
                        foreach (CMResult item in cmitems_out.Result)
                        {
                          await  ProcessCMSMSLog(item, session);
                        }
                    }

                    //CM Text log incoming
                    CMSMSResponse cmitems_in = await CMSMSLog(CMbatch.StartTime.UtcDateTime, CMbatch.EndTime.UtcDateTime, "in");
                    if (cmitems_in != null)
                    {
                        foreach (CMResult item in cmitems_in.Result)
                        {
                           await ProcessCMSMSLog(item, session);
                        }
                    }
                }

                if (twlbatch.PendingLogs > 0) {
                    List<MessageResource> LogsPush = new List<MessageResource>();
                    //Fetch Twilio logs
                    var items = MessageResource.Read(dateSentAfter: twlbatch.StartTime.ToUniversalTime().DateTime, dateSentBefore: twlbatch.EndTime.ToUniversalTime().DateTime);
                    foreach (var item in items) {
                        LogsPush.Add(item);
                       await ProcessSMSLog(item, session);
                    }
                   await SendLogDumpToApi("TEXT", null, LogsPush, null);
                }

                if (twlbatch.PendingLogs > 0 || CMbatch.PendingLogs > 0) {
                    //Fetch CM Logs.
                   await CreateCommsQueueSession(session);
                }

                //Console.ReadLine();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task ProcessCMSMSLog(CMResult item, string session)
        {
            try
            {

                //DBC.ModelInputLog("CommsLogHelper", "ProcessCMSMSLog", 16, 7, item);

                int tmpErrorCode = string.IsNullOrEmpty(item.Errordescription) ? 1 : 0;
                string ErrorCode = !string.IsNullOrEmpty(tmpErrorCode.ToString()) ? tmpErrorCode.ToString() : "NA";
                string ErrorMessage = !string.IsNullOrEmpty(item.Errordescription) ? item.Errordescription : "NA";
                int StatusVal = item.Status != null ? (int)item.Status : (int)19;
                string Status = CCConstants.CMSMSStatus(StatusVal);
                decimal Price = item.Price != null ? (decimal)item.Price : 0;
                int Segments = item.Multipart == null ? 1 : item.Multipart.Parts;
                DateTimeOffset DateSent = item.Deliverytime != null ? item.Created.AddSeconds(item.Deliverytime) : item.Created;

                string Direction = item.Direction.ToUpper() == "OUT" ? "outbound-api" : "inbound-api";

               await CreateCommsLogDump(session, item.Reference, "TEXT", Status, item.Sender, item.Recipient, Direction,
                                  Price, "self", item.Currency, Segments, item.Message, 0, item.Created, DateSent, DateSent,
                                  DateSent, ErrorCode, ErrorMessage, 0, commsProvider: "CM");

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<CMResult> GetCMSMS(string refId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://api.cmtelecom.com/v1.0/transactions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-CM-PRODUCTTOKEN", CM_CLIENTID);

                try {
                    //HTTP POST
                    var postTask = client.GetAsync("?pagesize=7500&reference=" + refId + "&startdate=" + startDate.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ss") + "&enddate=" + endDate.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss"));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode) {

                        Task<string> response = result.Content.ReadAsStringAsync();
                        string ressultstr = response.Result.Trim();

                        var objdata =  JsonConvert.DeserializeObject<CMSMSResponse>(ressultstr).Result;

                        if (objdata.Count > 0) {
                            CMResult rsp = objdata.FirstOrDefault();
                            return rsp;
                        }
                    }
                } catch (Exception ex) {
                    throw ex;
                }
            }
            return null;
        }

        public async Task DownloadAndCreateCMSMSLog(string cloudMessageId, DateTimeOffset createdOn)
        {
            try
            {
                string session = Guid.NewGuid().ToString();
                var message = await  GetCMSMS(cloudMessageId, createdOn.AddSeconds(-10), createdOn.AddSeconds(10));
                if (message != null)
                {
                   await  ProcessCMSMSLog(message, session);
                }
                var result = CreateCommsQueueSession(session);
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task DownloadAndCreateTwilioLog(string cloudMessageId, string method)
        {
            try
            {
                string session = Guid.NewGuid().ToString();
                TwilioClient.Init(PHONESID, PHONETOKEN);
                if (method.ToUpper() == "TEXT")
                {
                    var message = await GetTwilioMessage(cloudMessageId);
                    if (message != null)
                    {
                       await ProcessSMSLog(message, session);
                    }
                }
                else if (method.ToUpper() == "PHONE")
                {
                    var call = await GetTwilioCall(cloudMessageId);
                    if (call != null)
                    {
                       await ProcessCallLogs(call, session);
                    }
                }
               await  CreateCommsQueueSession(session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CMSMSResponse> CMSMSLog(DateTime startDate, DateTime endDate, string Direction)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://api.cmtelecom.com/v1.0/transactions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-CM-PRODUCTTOKEN", CM_CLIENTID);

                try {
                    //HTTP POST
                    var postTask = client.GetAsync("?pagesize=7500&direction=" + Direction + "&startdate=" + startDate.ToString("yyyy-MM-ddTHH:mm:ss") + "&enddate=" + endDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode) {

                        Task<string> response = result.Content.ReadAsStringAsync();
                        string ressultstr = response.Result.Trim();

                        var objdata = JsonConvert.DeserializeObject<CMSMSResponse>(ressultstr);

                        return objdata;
                    }
                } catch (Exception ex) {
                    throw ex;
                }
            }
            return null;
        }

        public async Task ProcessSMSLog(MessageResource item, string session)
        {
            try
            {
                int tmpErrorCode = item.ErrorCode != null ? (int)item.ErrorCode : 0;
                string ErrorCode = !string.IsNullOrEmpty(tmpErrorCode.ToString()) ? tmpErrorCode.ToString() : "NA";
                string ErrorMessage = !string.IsNullOrEmpty(item.ErrorMessage) ? item.ErrorMessage : "NA";
                string Price = item.Price != null ? item.Price : "0";

              await  CreateCommsLogDump(session, item.Sid, "TEXT", item.Status.ToString(), item.From.ToString(), item.To, item.Direction.ToString(),
                                  Convert.ToDecimal(Price), "self", item.PriceUnit, Convert.ToInt32(item.NumSegments), item.Body, 0, (DateTime)item.DateCreated,
                                  (DateTime)item.DateUpdated, (DateTime)item.DateSent, (DateTime)item.DateSent, ErrorCode, ErrorMessage, 0, "TWILIO");
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task DumpRecordings()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = await GetTwilioBatchTime("RECORDING", "TWILIO");
                int count = 1;

                if (twlbatch.PendingLogs > 0) {

                    var items =await RecordingResource.ReadAsync(dateCreatedAfter: twlbatch.StartTime.ToUniversalTime().DateTime, dateCreatedBefore: twlbatch.EndTime.ToUniversalTime().DateTime);
                    string session = Guid.NewGuid().ToString();

                    //DBC.CreateLog("INFO", "StarTime: " + twlbatch.StartTime.ToUniversalTime() + " | EndTime: " + twlbatch.EndTime.ToUniversalTime().DateTime, null, "", "DumpRecordings");

                    List<RecordingResource> LogsPush = new List<RecordingResource>();

                    foreach (var item in items) {
                        LogsPush.Add(item);
                        ProcessRecLog(item, session);
                        count++;
                    }

                   await  SendLogDumpToApi("RECORDING", null, null, LogsPush);

                   await CreateCommsQueueSession(session);
                }
                //Console.ReadLine();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task ProcessRecLog(RecordingResource item, string session)
        {
            try
            {
                decimal Price = item.Price == null ? 0M : Convert.ToDecimal(item.Price);
                int Duration = item.Duration == null ? 0 : Convert.ToInt32(item.Duration);

                await CreateCommsLog(item.Sid, "RECORDING", "COMPLETED", item.ConferenceSid, item.Source.ToString(), "outbound", Price, "", "USD", 0, "",
                     Duration, (DateTimeOffset)item.DateCreated, (DateTimeOffset)item.DateUpdated, (DateTimeOffset)item.DateCreated, (DateTimeOffset)item.DateCreated, commsProvider: "TWILIO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendLogDumpToApi(string logType, List<CallResource> calls, List<MessageResource> texts, List<RecordingResource> recs)
        {
            try
            {
                var pushlogshost = await db.Set<TwilioLogPushHost>().ToListAsync();

                foreach (var loghost in pushlogshost) {
                    if (!string.IsNullOrEmpty(loghost.LogCollectionUrl)) {
                        using (var client = new HttpClient()) {

                            client.BaseAddress = new Uri(loghost.LogCollectionUrl);

                            TwilioLogModel log = new TwilioLogModel();
                            log.LogType = logType;
                            log.Calls = calls;
                            log.Texts = texts;
                            log.Recordings = recs;
                            try
                            {
                                //HTTP POST
                                var result = client.PostAsJsonAsync("System/TwilioLogDump", log).Result;
                                if (result.IsSuccessStatusCode) {
                                    return true;
                                }
                            } catch (Exception ex) {
                                throw ex;
                            }
                        }
                    }
                }
                return false;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<bool> CreateCommsQueueSession(string sessionId)
        {
            try
            {
              await _queueHelper.CreateCommsLogDumpSession(sessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<bool> ProcessCommsLogs(string sessionId)
        {
            try
            {
                var pSession = new SqlParameter("@SessionId", sessionId);

                var result = await db.Database.ExecuteSqlRawAsync("Twilio_Log_Process @SessionId", pSession);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task CreateCommsLogDump(string session, string sid, string commType, string status, string from, string to, string direction, decimal price, string answeredBy,
         string priceUnit, int numSegments, string body, int duration, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset startTime,
         DateTimeOffset endTime, string errorCode = "", string errorMessage = "", int logStatus = 0, string commsProvider = "TWILIO")
        {
            try
            {

                var pSession = new SqlParameter("@SessionId", session);
                var pSid = new SqlParameter("@Sid", sid);
                var pDateCreated = new SqlParameter("@DateCreated", dateCreated);
                var pDateUpdated = new SqlParameter("@DateUpdated", dateUpdated);
                var pDateSent = new SqlParameter("@DateSent", startTime);
                var pBody = new SqlParameter("@Body", body);
                var pNumSegments = new SqlParameter("@NumSegments", numSegments);
                var pToFormatted = new SqlParameter("@ToFormatted", to);
                var pFromFormatted = new SqlParameter("@FromFormatted", from);
                var pStatus = new SqlParameter("@Status", status);
                var pStartTime = new SqlParameter("@StartTime", startTime);
                var pEndTime = new SqlParameter("@EndTime", endTime);
                var pDuration = new SqlParameter("@Duration", duration);
                var pPrice = new SqlParameter("@Price", price);
                var pPriceUnit = new SqlParameter("@PriceUnit", priceUnit);
                var pDirection = new SqlParameter("@Direction", direction);
                var pAnsweredBy = new SqlParameter("@AnsweredBy", answeredBy);
                var pErrorCode = new SqlParameter("@ErrorCode", errorCode);
                var pErrorMessage = new SqlParameter("@ErrorMessage", errorMessage);
                var pCommType = new SqlParameter("@CommType", commType);
                var pLogStatus = new SqlParameter("@LogStatus", logStatus);
                var pCommsProvider = new SqlParameter("@CommsPovider", commsProvider);


                var result = await  db.Database.ExecuteSqlRawAsync("Comms_Log_Dump_Insert @SessionId, @Sid, @DateCreated, @DateUpdated, @DateSent, @Body, @NumSegments, @ToFormatted, " +
                    "@FromFormatted, @Status, @StartTime, @EndTime, @Duration,@Price,@PriceUnit,@Direction,@AnsweredBy,@ErrorCode,@ErrorMessage,@CommType, @LogStatus,@CommsPovider",
                    pSession, pSid, pDateCreated, pDateUpdated, pDateSent, pBody, pNumSegments, pToFormatted, pFromFormatted, pStatus, pStartTime, pEndTime, pDuration, pPrice,
                    pPriceUnit, pDirection, pAnsweredBy, pErrorCode, pErrorMessage, pCommType, pLogStatus, pCommsProvider);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        

        public async Task<TwilioBatch> GetTwilioBatchTime(string logType, string commsProvider)
        {
            try {
                var pLogType = new SqlParameter("@LogType", logType);
                var pCommsProvider = new SqlParameter("@CommsProvider", commsProvider);

                var result = await db.Set<TwilioBatch>().FromSqlRaw("exec Get_Twilio_Log_Batch @LogType, @CommsProvider", pLogType, pCommsProvider).FirstOrDefaultAsync();
                if (result != null) {
                    return result;
                }
                return null;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<MessageResource> GetTwilioMessage(string sid)
        {
            if (!SendInDirect)
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);
                MessageResource message = null;
                try
                {
                    for (int i = 0; i < RetryCount; i++)
                    {
                        try
                        {
                            message = await MessageResource.FetchAsync(sid);
                            if (message != null)
                                break;
                        } catch (Exception ex) {
                            throw ex;
                        }
                    }

                } catch (Exception ex) {
                    throw ex;
                }
                return message;
            }
            else
            {
                return await GetTwilioMessageByApi(sid);
            }
        }

        public async Task<CallResource> GetTwilioCall(string sid)
        {
            if (!SendInDirect)
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);
                CallResource call = null;
                try
                {
                    for (int i = 0; i < RetryCount; i++)
                    {
                        try
                        {
                            call = await CallResource.FetchAsync(sid);
                            if (call != null)
                                break;
                        } catch (Exception ex) {
                            throw ex;
                        }
                    }

                } catch (Exception ex) {
                    throw ex;
                }
                return call;
            }
            else
            {
                return await GetTwilioCallByApi(sid);
            }
        }

        public async Task<ConferenceResource> GetTwilioConf(string sid)
        {
            if (!SendInDirect)
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);
                ConferenceResource conference = null;
                try
                {
                    for (int i = 0; i < RetryCount; i++)
                    {
                        try
                        {
                            conference = await ConferenceResource.FetchAsync(sid);
                            if (conference != null)
                                break;
                        } catch (Exception ex) {
                            throw ex;
                        }
                    }
                } catch (Exception ex) {
                    throw ex;
                }
                return conference;
            }
            else
            {
                return await GetTwilioConfByApi(sid);
            }
        }

        #region Api Based Fetching

        public async Task<MessageResource> GetTwilioMessageByApi(string sid)
        {
            MessageResource message = null;
            try {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest() {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioTextLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode) {
                    message = JsonConvert.DeserializeObject<MessageResource>(ressultstr);
                   await DBC.ModelInputLog("CommsLogs", "GetTwilioMessageByApi", 0, 0, message);
                }
                else
                {
                   await DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
            } catch (Exception ex) {
                throw ex;
            }
            return message;
        }

        public async Task<CallResource> GetTwilioCallByApi(string sid)
        {
            CallResource call = null;
            try {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest() {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioCallLog", req).Result;
                Task<string> resultstring =  RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode)
                {
                    call =  JsonConvert.DeserializeObject<CallResource>(ressultstr);
                   await DBC.ModelInputLog("CommsLogs", "GetTwilioCallByApi", 0, 0, call);
                }
                else
                {
                   await  DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return call;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<ConferenceResource> GetTwilioConfByApi(string sid)
        {
            ConferenceResource conf = null;
            try {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest() {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioConfLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode) {
                    conf = JsonConvert.DeserializeObject<ConferenceResource>(ressultstr);
                    await DBC.ModelInputLog("CommsLogs", "GetTwilioConfByApi", 0, 0, conf);
                }
                else
                {
                    await DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return conf;
            } catch (Exception ex) {
                throw ex;
            }

        }

        public async Task<ResourceSet<RecordingResource>> GetTwilioRecByApi(string sid)
        {
            ResourceSet<RecordingResource> conf = null;
            try {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest() {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioRecordingLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode && ressultstr.Length > 10) {
                    conf = JsonConvert.DeserializeObject<ResourceSet<RecordingResource>>(ressultstr);
                }
                else
                {
                   await  DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return conf;
            } catch (Exception ex) {
                throw ex;
            }

        }

        #endregion Api Based Fetching

        public async Task GetConferenceLog()
        {
            try
            {
                var confList = await db.Set<ConferenceCallLogHeader>().Where(CF => CF.CurrentStatus == "ENDED").ToListAsync();

                foreach (var conf in confList) {
                    try {

                        if (!string.IsNullOrEmpty(conf.CloudConfId)) {

                            ConferenceResource conference =await GetTwilioConf(conf.CloudConfId);

                            if (conference != null) {
                                if (conference.Status.ToString().ToUpper() == "COMPLETED") {
                                    conf.CloudConfId = conference.Sid;
                                    conf.ConfrenceStart = (DateTime)conference.DateCreated;
                                    conf.ConfrenceEnd = (DateTime)conference.DateUpdated;
                                    conf.CurrentStatus = conference.Status.ToString().ToUpper();
                                    conf.Duration = (int)((DateTime)conference.DateUpdated).Subtract(((DateTime)conference.DateCreated)).TotalSeconds;


                                    //Go To call list
                                    var confuser = await db.Set<ConferenceCallLogDetail>()
                                                    .Where(CD => CD.ConferenceCallId == conf.ConferenceCallId &&
                                                    CD.UserId == conf.InitiatedBy
                                                    ).ToListAsync();
                                    foreach (var rcpnt in confuser)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(rcpnt.SuccessCallId))
                                            {
                                                var call =await GetTwilioCall(rcpnt.SuccessCallId);

                                                if (call != null) {
                                                    //Create or update conference item record
                                                    string answered_by = call.AnsweredBy == null ? "" : call.AnsweredBy;
                                                    decimal Price = call.Price == null ? 0 : Convert.ToDecimal(call.Price);
                                                    int Duration = call.Duration == null ? 0 : Convert.ToInt32(call.Duration);

                                                    DateTimeOffset StartTime = call.StartTime == null ? conf.CreatedOn : (DateTimeOffset)call.StartTime;
                                                    DateTimeOffset EndTime = call.EndTime == null ? conf.CreatedOn : (DateTimeOffset)call.EndTime;

                                                   await CreateCommsLog(call.Sid, "CONFERENCE", call.Status.ToString(), call.From, call.To, call.Direction, Price, answered_by, "USD",
                                                        0, answered_by, Duration, (DateTimeOffset)call.DateCreated, (DateTimeOffset)call.DateUpdated, StartTime, EndTime, commsProvider: "TWILIO");

                                                    rcpnt.ConfJoined = StartTime;
                                                    rcpnt.ConfLeft = EndTime;
                                                    rcpnt.Status = call.Status.ToString().ToUpper();

                                                    //Get the recording of the call;
                                                  await  GetCallRecordingLog(call);
                                                }
                                            }
                                        } catch (Exception ex) {
                                            throw ex; ;
                                        }
                                    }
                                   await db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task GetCallRecordingLog(CallResource call)
        {
            try
            {
                ResourceSet<RecordingResource> recordings = null;
                if (!SendInDirect) {
                    TwilioClient.Init(PHONESID, PHONETOKEN);
                    recordings = RecordingResource.Read(callSid: call.Sid);
                }
                else
                {
                    recordings = await GetTwilioRecByApi(call.Sid);
                }

                if (recordings != null) {
                    foreach (var recording in recordings) {

                        decimal Price = recording.Price == null ? 0M : Convert.ToDecimal(recording.Price);
                        int Duration = recording.Duration == null ? 0 : Convert.ToInt32(recording.Duration);

                        DateTimeOffset StartTime = call.StartTime == null ? (DateTimeOffset)SqlDateTime.MinValue.Value : (DateTimeOffset)call.StartTime;
                        DateTimeOffset EndTime = call.EndTime == null ? (DateTimeOffset)SqlDateTime.MinValue.Value : (DateTimeOffset)call.EndTime;

                        await CreateCommsLog(recording.Sid, "RECORDING", "COMPLETED", call.From, call.To, call.Direction, Price, "", "USD", 0, "",
                             Duration, (DateTimeOffset)recording.DateCreated, (DateTimeOffset)recording.DateUpdated, StartTime, EndTime, commsProvider: "TWILIO");
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }


        public async Task GetCommsPrice()
        {
            try
            {
                //Get all the countries where voice and sms are supported
               await GetTwilioVoiceCountries();
               await GetTwilioSMSCountries();

                //get the price information for all the countries
              await  DownloadTwilioPricing();


            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task DownloadTwilioPricing()
        {
            try
            {
                var countries = await db.Set<Country>().Where(C => C.PhoneAvailable == true || C.Smsavailable == true && C.CountryPhoneCode != null).ToListAsync();
                foreach (var country in countries)
                {
                   await GetVoicePricing(country.Iso2code, country.CountryCode);
                   await GetSMSPricing(country.Iso2code, country.CountryCode, country.CountryPhoneCode);
                }

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task GetVoicePricing(string iso2Code, string countryCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(countryCode) && !string.IsNullOrEmpty(iso2Code))
                {

                    TwilioClient.Init(PHONESID, PHONETOKEN);

                    UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Getting voice prefix price information for country: " + countryCode));
                    FireUpdateEvent(UEA);

                    var country =await Twilio.Rest.Pricing.V1.Voice.CountryResource.FetchAsync(iso2Code);

                    UpdateEventArgs UEA2 = new UpdateEventArgs(DBC.LogWrite("Total " + country.OutboundPrefixPrices.Count + " prefix found for country " + countryCode));
                    FireUpdateEvent(UEA2);

                    foreach (var price in country.OutboundPrefixPrices)
                    {
                        foreach (var prefix in price.Prefixes)
                        {
                            var prfxrec = db.Set<TwilioPricing>().Where(TPR => TPR.CountryIso2 == iso2Code && TPR.DesinationPrefix == prefix && TPR.ChannelType == "PHONE").FirstOrDefault();
                            if (prfxrec != null)
                            {
                                prfxrec.CurrentPrice = Convert.ToDecimal(price.CurrentPrice);
                                prfxrec.BasePrice = Convert.ToDecimal(price.BasePrice);
                                prfxrec.FriendlyName = price.FriendlyName;
                                prfxrec.UpdateTime = DateTimeOffset.Now;
                                await db.SaveChangesAsync();

                                UpdateEventArgs UEA3 = new UpdateEventArgs(DBC.LogWrite("Voice price information updated for country " + countryCode + ": " + prefix));
                                FireUpdateEvent(UEA3);

                            }
                            else
                            {
                                await AddTwilioPrice(iso2Code, countryCode, "PHONE", prefix, Convert.ToDecimal(price.BasePrice), Convert.ToDecimal(price.CurrentPrice),
                                     price.FriendlyName);

                                UpdateEventArgs UEA4 = new UpdateEventArgs(DBC.LogWrite("Voice price information added for country " + countryCode + ": " + prefix));
                                FireUpdateEvent(UEA4);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task GetSMSPricing(string iso2Code, string countryCode, string dialingCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(dialingCode) && !string.IsNullOrEmpty(iso2Code))
                {
                    TwilioClient.Init(PHONESID, PHONETOKEN);
                    UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Getting voice prefix price information for country: " + countryCode));
                    FireUpdateEvent(UEA);

                    var country = Twilio.Rest.Pricing.V1.Messaging.CountryResource.Fetch(iso2Code);

                    UpdateEventArgs UEA2 = new UpdateEventArgs(DBC.LogWrite("Total " + country.OutboundSmsPrices.Count + " prefix found for country " + countryCode));
                    FireUpdateEvent(UEA2);

                    foreach (var carrier in country.OutboundSmsPrices) {
                        foreach (var price in carrier.Prices) {
                            string numberType = price.NumberType.ToString();
                            var prfxrec = db.Set<TwilioPricing>()
                                           .Where(TPR => TPR.CountryIso2 == iso2Code && TPR.FriendlyName == carrier.Carrier
                                           && TPR.ChannelType == "SMS" && TPR.NumberType == numberType
                                           ).FirstOrDefault();
                            if (prfxrec != null) {
                                prfxrec.CurrentPrice = Convert.ToDecimal(price.CurrentPrice);
                                prfxrec.BasePrice = Convert.ToDecimal(price.BasePrice);
                                prfxrec.FriendlyName = carrier.Carrier;
                                prfxrec.NumberType = price.NumberType.ToString();
                                prfxrec.UpdateTime = DateTimeOffset.Now;
                                await db.SaveChangesAsync();

                                int PriceID = prfxrec.Id;
                                UpdateEventArgs UEA3 = new UpdateEventArgs(DBC.LogWrite(PriceID + ": SMS price information updated for country " + countryCode + ": " + dialingCode));
                                FireUpdateEvent(UEA3);

                            }
                            else
                            {
                                int PriceID = await AddTwilioPrice(iso2Code, countryCode, "SMS", dialingCode, Convert.ToDecimal(price.BasePrice), Convert.ToDecimal(price.CurrentPrice),
                                    carrier.Carrier, price.NumberType.ToString());

                                UpdateEventArgs UEA4 = new UpdateEventArgs(DBC.LogWrite(PriceID + ": SMS price information added for country " + countryCode + ": " + dialingCode));
                                FireUpdateEvent(UEA4);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<int> AddTwilioPrice(string iso2Code, string iso3Code, string channelType, string prefix, decimal basePrice, decimal currentPrice, string friendyName, string numberType = "")
        {
            try
            {
                TwilioPricing TP = new TwilioPricing();
                TP.BasePrice = basePrice;
                TP.ChannelType = channelType;
                TP.CountryCode = iso3Code;
                TP.CountryIso2 = iso2Code;
                TP.CurrentPrice = currentPrice;
                TP.DesinationPrefix = prefix;
                TP.FriendlyName = friendyName;
                TP.NumberType = numberType;
                TP.UpdateTime = DateTimeOffset.Now;
                await db.AddAsync(TP);
                await db.SaveChangesAsync();
                return TP.Id;
            } catch (Exception ex) {
                throw ex;
                return 0;
            }
        }

        public async Task GetTwilioVoiceCountries()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                var countries = await  Twilio.Rest.Pricing.V1.Voice.CountryResource.ReadAsync(300);

                foreach (var country in countries)
                {
                    var cntr = await db.Set<Country>().Where(C => C.Iso2code == country.IsoCountry).FirstOrDefaultAsync();
                    if (cntr != null)
                    {
                        cntr.PhoneAvailable = true;
                        cntr.VoicePriceUrl = country.Url.ToString();

                        UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Country " + cntr.Name + " voice api url is updated"));
                        FireUpdateEvent(UEA);
                    }
                }
               await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task GetTwilioSMSCountries()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);
                var countries = await Twilio.Rest.Pricing.V1.Messaging.CountryResource.ReadAsync(500);
                foreach (var country in countries)
                {
                    var cntr =await db.Set<Country>().Where(C => C.Iso2code == country.IsoCountry).FirstOrDefaultAsync();
                    if (cntr != null)
                    {
                        cntr.Smsavailable = true;
                        cntr.SmspriceUrl = country.Url.ToString();
                        UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Country " + cntr.Name + " SMS api url is updated"));
                        FireUpdateEvent(UEA);
                    }
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task UpdateBalance_SMS(string smsSid)
        {
            try
            {
                var message =await GetTwilioMessage(smsSid);

                if (message != null) {

                    int tmpErrorCode = message.ErrorCode != null ? (int)message.ErrorCode : 0;
                    string ErrorCode = !string.IsNullOrEmpty(tmpErrorCode.ToString()) ? tmpErrorCode.ToString() : "NA";
                    string ErrorMessage = !string.IsNullOrEmpty(message.ErrorMessage) ? message.ErrorMessage : "NA";
                    string Price = message.Price != null ? message.Price : "0";

                    await CreateCommsLog(message.Sid, "TEXT", message.Status.ToString(), message.From.ToString(), message.To, message.Direction.ToString(),
                         Convert.ToDecimal(Price), "self", message.PriceUnit, Convert.ToInt32(message.NumSegments), message.Body, 0, (DateTime)message.DateCreated,
                         (DateTime)message.DateUpdated, (DateTime)message.DateSent, (DateTime)message.DateSent, ErrorCode, ErrorMessage, commsProvider: "TWILIO");
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task UpdateBalance_PHONE(string callSid)
        {
            try
            {
                var call =await GetTwilioCall(callSid);

                if (call != null) {

                    decimal Price = call.Price == null ? 0 : Convert.ToDecimal(call.Price);
                    int Duration = call.Duration == null ? 0 : Convert.ToInt32(call.Duration);
                    string answered_by = call.AnsweredBy == null ? "" : call.AnsweredBy;

                    DateTime StartTime = call.StartTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)call.StartTime;
                    DateTime EndTime = call.EndTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)call.EndTime;

                    await CreateCommsLog(call.Sid, "PHONE", call.Status.ToString(), call.From, call.To, call.Direction, Price, answered_by, "USD", 0, "", Duration,
                               (DateTime)call.DateCreated, (DateTime)call.DateUpdated, StartTime, EndTime, commsProvider: "TWILIO");

                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<TwilioPriceByNumber> GetLocalPrice(string phoneNumber, string channel)
        {
            TwilioPriceByNumber tp = new TwilioPriceByNumber
            {
                Price = 0
            };
            try
            {
                var pPhoneNumber = new SqlParameter("@PhoneNumber", phoneNumber);
                var pChannel = new SqlParameter("@Channel", channel);

                tp =await db.Set<TwilioPriceByNumber>().FromSqlRaw("exec Get_Twilio_Pricing_By_Number @PhoneNumber, @Channel", pPhoneNumber, pChannel).FirstOrDefaultAsync();
                if (tp != null)
                {
                    return tp;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tp;
        }

        public async Task ClearTwilioLogs(int companyID)
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                List<TwilioLogToClear> logs = await GetTwilioLogsToClear(companyID);
                foreach (TwilioLogToClear log in logs)
                {
                    if (log.MethodName.ToUpper() == "TEXT")
                    {
                        try
                        {
                            MessageResource.Delete(log.Sid);
                        } catch (Exception) {
                        }

                    } else if (log.MethodName.ToUpper() == "PHONE") {
                        try {
                            CallResource.Delete(log.Sid);
                        } catch (Exception) {
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<TwilioLogToClear>> GetTwilioLogsToClear(int CompanyID)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await db.Set<TwilioLogToClear>().FromSqlRaw("exec Get_Twilio_Logs_To_Clear @CompanyID", pCompanyID).ToListAsync();
                return result;


            } catch (Exception ex) {
                throw ex;
            }
            return null;
        }

        public async Task CreateCommsLog(string sid, string commType, string status, string from, string to, string direction, decimal price, string answeredBy, string priceUnit,
            int numSegments, string body, int duration, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset startTime, DateTimeOffset endTime,
            string errorCode = "", string errorMessage = "", string commsProvider = "") {
            try {
                if (!string.IsNullOrEmpty(sid)) {
                    var checksid = await (from L in db.Set<CommsLog>() where L.Sid == sid select L).FirstOrDefaultAsync();
                    if (checksid != null)
                    {
                        checksid.DateCreated = await DBC.ToNullIfTooEarlyForDb(dateCreated);
                        checksid.Price = price;
                        checksid.Status = status;
                        checksid.Duration = duration;
                        checksid.DateUpdated = await DBC.ToNullIfTooEarlyForDb(dateUpdated);
                        checksid.EndTime = endTime != null ? await DBC.ToNullIfTooEarlyForDb(endTime) : dateUpdated;
                        checksid.CommType = commType;
                        checksid.NumSegments = numSegments;
                        checksid.Body = body;
                        checksid.DateSent = startTime != null ? await DBC.ToNullIfTooEarlyForDb(startTime) : dateUpdated;
                        checksid.StartTime = endTime != null ? await DBC.ToNullIfTooEarlyForDb(endTime) : dateUpdated;
                        checksid.ErrorCode = !string.IsNullOrEmpty(errorCode) ? errorCode : "";
                        checksid.ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : "";
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        CommsLog CL = new CommsLog();
                        CL.Sid = sid;
                        CL.DateCreated = await DBC.ToNullIfTooEarlyForDb(dateCreated);
                        CL.DateUpdated = await DBC.ToNullIfTooEarlyForDb(dateUpdated);
                        CL.DateSent = await DBC.ToNullIfTooEarlyForDb(startTime);
                        CL.Body = body;
                        CL.NumSegments = numSegments;
                        CL.ToFormatted = to;
                        CL.FromFormatted = from;
                        CL.Status = status;
                        CL.StartTime = startTime != null ? await DBC.ToNullIfTooEarlyForDb(startTime) : dateUpdated;
                        CL.EndTime = endTime != null ? await DBC.ToNullIfTooEarlyForDb(endTime) : dateUpdated;
                        CL.Duration = duration;
                        CL.Price = price;
                        CL.PriceUnit = priceUnit;
                        CL.Direction = direction;
                        CL.AnsweredBy = answeredBy;
                        CL.ErrorCode = !string.IsNullOrEmpty(errorCode) ? errorCode : "";
                        CL.ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : "";
                        CL.CommType = commType;
                        CL.CommsProvider = commsProvider;
                        await db.Set<CommsLog>().AddAsync(CL);
                        await db.SaveChangesAsync();
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
