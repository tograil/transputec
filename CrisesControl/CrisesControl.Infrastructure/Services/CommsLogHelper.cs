using CrisesControl.Api.Application.Helpers;
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
using static CrisesControl.Api.Application.Helpers.DBCommon;
using Twilio.Base;

namespace CrisesControl.Infrastructure.Services
{
    public class CommsLogsHelper
    {
        private readonly CrisesControlContext db;
        private readonly DBCommon DBC;
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

        public CommsLogsHelper(CrisesControlContext db, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this._httpContextAccessor = httpContextAccessor;
            this.DBC = new DBCommon(db,_httpContextAccessor);
        }

        public CommsLogsHelper()
        {
            PHONESID = DBC.LookupWithKey("PHONESID");
            PHONETOKEN = DBC.LookupWithKey("PHONETOKEN");
            CM_CLIENTID = DBC.LookupWithKey("CM_CLIENTID");
            //bool log_age = Double.TryParse(DBC.LookupWithKey("COMMS_LOG_AGE_HOURS"), out COMMS_LOG_AGE_HOURS);
            //RECORDING_URL = DBC.LookupWithKey("RECORDING_URL");

            TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

            int.TryParse(DBC.LookupWithKey("TWILIO_MESSAGE_RETRY_COUNT"), out RetryCount);
            string sendDirect = DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION");

            SendInDirect = DBC.IsTrue(sendDirect, false);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }
        public void FireUpdateEvent(UpdateEventArgs e)
        {
            EntityUpdate?.Invoke(this, e);
        }

        public void GetCommsLogs()
        {
            //Create function to get twilio log batch interval
            if (!SendInDirect)
            {
                //DBC.CreateLog("INFO", "Running the functions just now");
                DumpCallLogs();
                DumpSMSLogs();
                DumpRecordings();
            }
            //GetTwilioSMSLog();
            //GetTwilioCallLog();
            GetConferenceLog();
            //GetTwoFactorAuthLog();
        }

        public void GetCommsLogsForced()
        {
            ForceLogCollection();
        }

        public void ForceLogCollection()
        {
            try
            {
                string session = Guid.NewGuid().ToString();

                TwilioClient.Init(PHONESID, PHONETOKEN);

                DateTimeOffset checkTime = DBC.GetDateTimeOffset(DateTime.Now.AddHours(-2));

                var getSMSList = (from MT in db.Set<MessageTransaction>()
                                  where MT.CloudMessageId != null && (MT.MethodName == "TEXT" || MT.MethodName == "PHONE") &&
                                  MT.LogCollected == false
                                  select MT).ToList();

                foreach (var msg in getSMSList)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(msg.CloudMessageId))
                        {

                            //if(DateTimeOffset.Now.Subtract((DateTimeOffset)msg.CreatedOn).TotalHours > 2 && msg.CloudMessageId.Length > 30) {

                            if (msg.CommsProvider == "TWILIO")
                            {
                                if (msg.MethodName.ToUpper() == "TEXT")
                                {
                                    var message = GetTwilioMessage(msg.CloudMessageId);
                                    if (message != null)
                                    {
                                        ProcessSMSLog(message, session);
                                    }
                                }
                                else if (msg.MethodName.ToUpper() == "PHONE")
                                {
                                    var call = GetTwilioCall(msg.CloudMessageId);
                                    if (call != null)
                                    {
                                        ProcessCallLogs(call, session);
                                    }
                                }
                            }
                            else if (msg.CommsProvider == "CM")
                            {
                                var sms = GetCMSMS(msg.CloudMessageId, msg.CreatedOn.Value.AddSeconds(-10), msg.CreatedOn.Value.AddSeconds(10));
                                if (sms != null)
                                {
                                    ProcessCMSMSLog(sms, session);
                                }
                            }
                            //}
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                var result = CreateCommsQueueSession(session);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DumpCallLogs()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = GetTwilioBatchTime("PHONE", "TWILIO");
                int count = 1;

                //DBC.CreateLog("INFO", "Running the Call Logs data collecton");

                if (twlbatch.PendingLogs > 0)
                {
                    var items = CallResource.Read(endTimeAfter: twlbatch.StartTime.ToUniversalTime().DateTime, endTimeBefore: twlbatch.EndTime.ToUniversalTime().DateTime);

                    string session = Guid.NewGuid().ToString();

                    //DBC.CreateLog("INFO", "StarTime: " + twlbatch.StartTime.ToUniversalTime().DateTime + " | EndTime: " + twlbatch.EndTime.ToUniversalTime().DateTime, null, "", "DumpCallLogs");

                    List<CallResource> LogsPush = new List<CallResource>();

                    foreach (var item in items)
                    {
                        LogsPush.Add(item);
                        Console.WriteLine("Message count" + count + " => " + item.Sid);
                        ProcessCallLogs(item, session);
                        count++;
                    }
                    //todo create a memory object of items and push it to MEA like recipeints.
                    //DBC.CreateLog("INFO", "Total Items: " + LogsPush.Count);

                    SendLogDumpToApi("PHONE", LogsPush, null, null);

                    CreateCommsQueueSession(session);
                }
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcessCallLogs(CallResource item, string session)
        {
            try
            {
                decimal Price = item.Price == null ? 0 : Convert.ToDecimal(item.Price);
                int Duration = item.Duration == null ? 0 : Convert.ToInt32(item.Duration);
                string answered_by = item.AnsweredBy == null ? "self" : item.AnsweredBy;

                DateTime StartTime = item.StartTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)item.StartTime;
                DateTime EndTime = item.EndTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)item.EndTime;

                CreateCommsLogDump(session, item.Sid, "PHONE", item.Status.ToString(), item.From, item.To, item.Direction, Price, answered_by, "USD", 0, "", Duration,
                        (DateTime)item.DateCreated, (DateTime)item.DateUpdated, StartTime, EndTime, CommsProvider: "TWILIO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DumpSMSLogs()
        {
            try
            {

                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = GetTwilioBatchTime("TEXT", "TWILIO");
                TwilioBatch CMbatch = GetTwilioBatchTime("TEXT", "CM");

                string session = Guid.NewGuid().ToString();

                if (twlbatch.PendingLogs > 0)
                {

                    //CM Text log outgoing
                    CMSMSResponse cmitems_out = CMSMSLog(CMbatch.StartTime.UtcDateTime, CMbatch.EndTime.UtcDateTime, "out");
                    if (cmitems_out != null)
                    {
                        foreach (CMResult item in cmitems_out.Result)
                        {
                            ProcessCMSMSLog(item, session);
                        }
                    }

                    //CM Text log incoming
                    CMSMSResponse cmitems_in = CMSMSLog(CMbatch.StartTime.UtcDateTime, CMbatch.EndTime.UtcDateTime, "in");
                    if (cmitems_in != null)
                    {
                        foreach (CMResult item in cmitems_in.Result)
                        {
                            ProcessCMSMSLog(item, session);
                        }
                    }
                }

                if (twlbatch.PendingLogs > 0)
                {
                    List<MessageResource> LogsPush = new List<MessageResource>();
                    //Fetch Twilio logs
                    var items = MessageResource.Read(dateSentAfter: twlbatch.StartTime.ToUniversalTime().DateTime, dateSentBefore: twlbatch.EndTime.ToUniversalTime().DateTime);
                    foreach (var item in items)
                    {
                        LogsPush.Add(item);
                        ProcessSMSLog(item, session);
                    }
                    SendLogDumpToApi("TEXT", null, LogsPush, null);
                }

                if (twlbatch.PendingLogs > 0 || CMbatch.PendingLogs > 0)
                {
                    //Fetch CM Logs.
                    CreateCommsQueueSession(session);
                }

                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcessCMSMSLog(CMResult item, string session)
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

                CreateCommsLogDump(session, item.Reference, "TEXT", Status, item.Sender, item.Recipient, Direction,
                                  Price, "self", item.Currency, Segments, item.Message, 0, item.Created, DateSent, DateSent,
                                  DateSent, ErrorCode, ErrorMessage, 0, CommsProvider: "CM");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CMResult GetCMSMS(string refId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://api.cmtelecom.com/v1.0/transactions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-CM-PRODUCTTOKEN", CM_CLIENTID);

                try
                {
                    //HTTP POST
                    var postTask = client.GetAsync("?pagesize=7500&reference=" + refId + "&startdate=" + startDate.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ss") + "&enddate=" + endDate.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss"));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        Task<string> response = result.Content.ReadAsStringAsync();
                        string ressultstr = response.Result.Trim();

                        var objdata = JsonConvert.DeserializeObject<CMSMSResponse>(ressultstr).Result;

                        if (objdata.Count > 0)
                        {
                            CMResult rsp = objdata.FirstOrDefault();
                            return rsp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        public void DownloadAndCreateCMSMSLog(string CloudMessageId, DateTimeOffset CreatedOn)
        {
            try
            {
                string session = Guid.NewGuid().ToString();
                var message = GetCMSMS(CloudMessageId, CreatedOn.AddSeconds(-10), CreatedOn.AddSeconds(10));
                if (message != null)
                {
                    ProcessCMSMSLog(message, session);
                }
                var result = CreateCommsQueueSession(session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DownloadAndCreateTwilioLog(string CloudMessageId, string Method)
        {
            try
            {
                string session = Guid.NewGuid().ToString();
                TwilioClient.Init(PHONESID, PHONETOKEN);
                if (Method.ToUpper() == "TEXT")
                {
                    var message = GetTwilioMessage(CloudMessageId);
                    if (message != null)
                    {
                        ProcessSMSLog(message, session);
                    }
                }
                else if (Method.ToUpper() == "PHONE")
                {
                    var call = GetTwilioCall(CloudMessageId);
                    if (call != null)
                    {
                        ProcessCallLogs(call, session);
                    }
                }
                CreateCommsQueueSession(session);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CMSMSResponse CMSMSLog(DateTime startDate, DateTime endDate, string Direction)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://api.cmtelecom.com/v1.0/transactions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-CM-PRODUCTTOKEN", CM_CLIENTID);

                try
                {
                    //HTTP POST
                    var postTask = client.GetAsync("?pagesize=7500&direction=" + Direction + "&startdate=" + startDate.ToString("yyyy-MM-ddTHH:mm:ss") + "&enddate=" + endDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        Task<string> response = result.Content.ReadAsStringAsync();
                        string ressultstr = response.Result.Trim();

                        var objdata = JsonConvert.DeserializeObject<CMSMSResponse>(ressultstr);

                        return objdata;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        public void ProcessSMSLog(MessageResource item, string session)
        {
            try
            {
                int tmpErrorCode = item.ErrorCode != null ? (int)item.ErrorCode : 0;
                string ErrorCode = !string.IsNullOrEmpty(tmpErrorCode.ToString()) ? tmpErrorCode.ToString() : "NA";
                string ErrorMessage = !string.IsNullOrEmpty(item.ErrorMessage) ? item.ErrorMessage : "NA";
                string Price = item.Price != null ? item.Price : "0";

                CreateCommsLogDump(session, item.Sid, "TEXT", item.Status.ToString(), item.From.ToString(), item.To, item.Direction.ToString(),
                                  Convert.ToDecimal(Price), "self", item.PriceUnit, Convert.ToInt32(item.NumSegments), item.Body, 0, (DateTime)item.DateCreated,
                                  (DateTime)item.DateUpdated, (DateTime)item.DateSent, (DateTime)item.DateSent, ErrorCode, ErrorMessage, 0, "TWILIO");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void DumpRecordings()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                TwilioBatch twlbatch = GetTwilioBatchTime("RECORDING", "TWILIO");
                int count = 1;

                if (twlbatch.PendingLogs > 0)
                {

                    var items = RecordingResource.Read(dateCreatedAfter: twlbatch.StartTime.ToUniversalTime().DateTime, dateCreatedBefore: twlbatch.EndTime.ToUniversalTime().DateTime);
                    string session = Guid.NewGuid().ToString();

                    //DBC.CreateLog("INFO", "StarTime: " + twlbatch.StartTime.ToUniversalTime() + " | EndTime: " + twlbatch.EndTime.ToUniversalTime().DateTime, null, "", "DumpRecordings");

                    List<RecordingResource> LogsPush = new List<RecordingResource>();

                    foreach (var item in items)
                    {
                        LogsPush.Add(item);
                        ProcessRecLog(item, session);
                        count++;
                    }

                    SendLogDumpToApi("RECORDING", null, null, LogsPush);

                    CreateCommsQueueSession(session);
                }
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void ProcessRecLog(RecordingResource item, string session)
        {
            try
            {
                decimal Price = item.Price == null ? 0M : Convert.ToDecimal(item.Price);
                int Duration = item.Duration == null ? 0 : Convert.ToInt32(item.Duration);

               await CreateCommsLogAsync(item.Sid, "RECORDING", "COMPLETED", item.ConferenceSid, item.Source.ToString(), "outbound", Price, "", "USD", 0, "",
                    Duration, (DateTimeOffset)item.DateCreated, (DateTimeOffset)item.DateUpdated, (DateTimeOffset)item.DateCreated, (DateTimeOffset)item.DateCreated, CommsProvider: "TWILIO");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public class TwilioLogModel
        //{
        //    public string LogType { get; set; }
        //    public List<CallResource> Calls { get; set; }
        //    public List<MessageResource> Texts { get; set; }
        //    public List<RecordingResource> Recordings { get; set; }
        //}

        public bool SendLogDumpToApi(string LogType, List<CallResource> Calls, List<MessageResource> Texts, List<RecordingResource> Recs)
        {
            try
            {
                var pushlogshost =  db.Set<TwilioLogPushHost>().ToList();

                foreach (var loghost in pushlogshost)
                {
                    if (!string.IsNullOrEmpty(loghost.LogCollectionUrl))
                    {
                        using (var client = new HttpClient())
                        {

                            client.BaseAddress = new Uri(loghost.LogCollectionUrl);

                            TwilioLogModel log = new TwilioLogModel();
                            log.LogType = LogType;
                            log.Calls = Calls;
                            log.Texts = Texts;
                            log.Recordings = Recs;
                            try
                            {
                                //HTTP POST
                                var result = client.PostAsJsonAsync("System/TwilioLogDump", log).Result;
                                if (result.IsSuccessStatusCode)
                                {
                                    return true;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public bool CreateCommsQueueSession(string SessionId)
        {
            try
            {
                QueueHelper.CreateCommsLogDumpSession(SessionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public bool ProcessCommsLogs(string SessionId)
        {
            try
            {
                var pSession = new SqlParameter("@SessionId", SessionId);

                 var result = db.Database.ExecuteSqlRaw("Twilio_Log_Process @SessionId", pSession);
                    return true;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public void CreateCommsLogDump(string Session, string Sid, string CommType, string Status, string From, string To, string Direction, decimal Price, string AnsweredBy,
         string PriceUnit, int NumSegments, string Body, int Duration, DateTimeOffset DateCreated, DateTimeOffset DateUpdated, DateTimeOffset StartTime,
         DateTimeOffset EndTime, string ErrorCode = "", string ErrorMessage = "", int LogStatus = 0, string CommsProvider = "TWILIO")
        {
            try
            {
                //CommsLogDumpPayload payload = new CommsLogDumpPayload();
                //payload.Session = Session;
                //payload.Sid = Sid;
                //payload.CommType = CommType;
                //payload.Status = Status;
                //payload.FromFormatted = From;
                //payload.ToFormatted = To;
                //payload.Direction = Direction;
                //payload.Price = Price;
                //payload.AnsweredBy = AnsweredBy;
                //payload.PriceUnit = PriceUnit;
                //payload.NumSegments = NumSegments;
                //payload.Body = Body;
                //payload.Duration = Duration;
                //payload.DateCreated = DateCreated;
                //payload.DateUpdated = DateUpdated;
                //payload.StartTime = StartTime;
                //payload.EndTime = EndTime;
                //payload.ErrorCode = ErrorCode;
                //payload.ErrorMessage = ErrorMessage;
                //payload.LogStatus = LogStatus;
                //payload.CommsProvider = CommsProvider;
                //payload.DateSent = StartTime;

                var pSession = new SqlParameter("@SessionId", Session);
                var pSid = new SqlParameter("@Sid", Sid);
                var pDateCreated = new SqlParameter("@DateCreated", DateCreated);
                var pDateUpdated = new SqlParameter("@DateUpdated", DateUpdated);
                var pDateSent = new SqlParameter("@DateSent", StartTime);
                var pBody = new SqlParameter("@Body", Body);
                var pNumSegments = new SqlParameter("@NumSegments", NumSegments);
                var pToFormatted = new SqlParameter("@ToFormatted", To);
                var pFromFormatted = new SqlParameter("@FromFormatted", From);
                var pStatus = new SqlParameter("@Status", Status);
                var pStartTime = new SqlParameter("@StartTime", StartTime);
                var pEndTime = new SqlParameter("@EndTime", EndTime);
                var pDuration = new SqlParameter("@Duration", Duration);
                var pPrice = new SqlParameter("@Price", Price);
                var pPriceUnit = new SqlParameter("@PriceUnit", PriceUnit);
                var pDirection = new SqlParameter("@Direction", Direction);
                var pAnsweredBy = new SqlParameter("@AnsweredBy", AnsweredBy);
                var pErrorCode = new SqlParameter("@ErrorCode", ErrorCode);
                var pErrorMessage = new SqlParameter("@ErrorMessage", ErrorMessage);
                var pCommType = new SqlParameter("@CommType", CommType);
                var pLogStatus = new SqlParameter("@LogStatus", LogStatus);
                var pCommsProvider = new SqlParameter("@CommsPovider", CommsProvider);

                
                    var result = db.Database.ExecuteSqlRaw("Comms_Log_Dump_Insert @SessionId, @Sid, @DateCreated, @DateUpdated, @DateSent, @Body, @NumSegments, @ToFormatted, " +
                        "@FromFormatted, @Status, @StartTime, @EndTime, @Duration,@Price,@PriceUnit,@Direction,@AnsweredBy,@ErrorCode,@ErrorMessage,@CommType, @LogStatus,@CommsPovider",
                        pSession, pSid, pDateCreated, pDateUpdated, pDateSent, pBody, pNumSegments, pToFormatted, pFromFormatted, pStatus, pStartTime, pEndTime, pDuration, pPrice,
                        pPriceUnit, pDirection, pAnsweredBy, pErrorCode, pErrorMessage, pCommType, pLogStatus, pCommsProvider);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public class TwilioBatch
        {
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public int PendingLogs { get; set; }
        }

        public TwilioBatch GetTwilioBatchTime(string LogType, string CommsProvider)
        {
            try
            {
                var pLogType = new SqlParameter("@LogType", LogType);
                var pCommsProvider = new SqlParameter("@CommsProvider", CommsProvider);

               var result = db.Set<TwilioBatch>().FromSqlRaw("exec Get_Twilio_Log_Batch @LogType, @CommsProvider", pLogType, pCommsProvider).FirstOrDefault();
                    if (result != null)
                    {
                        return result;
                    }
                return null;

            }
            catch (Exception ex)
            {
                throw ex ;
            }
            
        }

        public MessageResource GetTwilioMessage(string Sid)
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
                            message = MessageResource.Fetch(Sid);
                            if (message != null)
                                break;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                } 
                return message;
            }
            else
            {
                return GetTwilioMessageByApi(Sid);
            }
        }

        public CallResource GetTwilioCall(string Sid)
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
                            call = CallResource.Fetch(Sid);
                            if (call != null)
                                break;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return call;
            }
            else
            {
                return GetTwilioCallByApi(Sid);
            }
        }

        private ConferenceResource GetTwilioConf(string Sid)
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
                            conference = ConferenceResource.Fetch(Sid);
                            if (conference != null)
                                break;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return conference;
            }
            else
            {
                return GetTwilioConfByApi(Sid);
            }
        }

        #region Api Based Fetching

        public MessageResource GetTwilioMessageByApi(string Sid)
        {
            MessageResource message = null;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest()
                {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = Sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioTextLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode)
                {
                    message = JsonConvert.DeserializeObject<MessageResource>(ressultstr);
                    DBC.ModelInputLog("CommsLogs", "GetTwilioMessageByApi", 0, 0, message);
                }
                else
                {
                    DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        public CallResource GetTwilioCallByApi(string Sid)
        {
            CallResource call = null;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest()
                {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = Sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioCallLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode)
                {
                    call = JsonConvert.DeserializeObject<CallResource>(ressultstr);
                    DBC.ModelInputLog("CommsLogs", "GetTwilioCallByApi", 0, 0, call);
                }
                else
                {
                    DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return call;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        public ConferenceResource GetTwilioConfByApi(string Sid)
        {
            ConferenceResource conf = null;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest()
                {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = Sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioConfLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode)
                {
                    conf = JsonConvert.DeserializeObject<ConferenceResource>(ressultstr);
                    DBC.ModelInputLog("CommsLogs", "GetTwilioConfByApi", 0, 0, conf);
                }
                else
                {
                    DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return conf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ResourceSet<RecordingResource> GetTwilioRecByApi(string Sid)
        {
            ResourceSet<RecordingResource> conf = null;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(TwilioRoutingApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var req = new TwilioRequest()
                {
                    ClientId = PHONESID,
                    Secret = PHONETOKEN,
                    Sid = Sid,
                    RetryCount = RetryCount
                };
                HttpResponseMessage RspApi = client.PostAsJsonAsync("Communication/TwilioRecordingLog", req).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result;

                if (RspApi.IsSuccessStatusCode && ressultstr.Length > 10)
                {
                    conf = JsonConvert.DeserializeObject<ResourceSet<RecordingResource>>(ressultstr);
                }
                else
                {
                    DBC.CreateLog("INFO", ressultstr, null, "CommsClient", "ApiCall", 0);
                }
                return conf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        #endregion Api Based Fetching

        public void GetConferenceLog()
        {
            try
            {
                var confList = db.Set<ConferenceCallLogHeader>().Where(CF=> CF.CurrentStatus == "ENDED" ).ToList();

                foreach (var conf in confList)
                {
                    try
                    {

                        if (!string.IsNullOrEmpty(conf.CloudConfId))
                        {

                            ConferenceResource conference = GetTwilioConf(conf.CloudConfId);

                            if (conference != null)
                            {
                                if (conference.Status.ToString().ToUpper() == "COMPLETED")
                                {
                                    conf.CloudConfId = conference.Sid;
                                    conf.ConfrenceStart = (DateTime)conference.DateCreated;
                                    conf.ConfrenceEnd = (DateTime)conference.DateUpdated;
                                    conf.CurrentStatus = conference.Status.ToString().ToUpper();
                                    conf.Duration = (int)((DateTime)conference.DateUpdated).Subtract(((DateTime)conference.DateCreated)).TotalSeconds;


                                    //Go To call list
                                    var confuser =  db.Set<ConferenceCallLogDetail>()
                                                    .Where(CD=> CD.ConferenceCallId == conf.ConferenceCallId &&
                                                    CD.UserId == conf.InitiatedBy
                                                    ).ToList();
                                    foreach (var rcpnt in confuser)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(rcpnt.SuccessCallId))
                                            {
                                                var call = GetTwilioCall(rcpnt.SuccessCallId);

                                                if (call != null)
                                                {
                                                    //Create or update conference item record
                                                    string answered_by = call.AnsweredBy == null ? "" : call.AnsweredBy;
                                                    decimal Price = call.Price == null ? 0 : Convert.ToDecimal(call.Price);
                                                    int Duration = call.Duration == null ? 0 : Convert.ToInt32(call.Duration);

                                                    DateTimeOffset StartTime = call.StartTime == null ? conf.CreatedOn : (DateTimeOffset)call.StartTime;
                                                    DateTimeOffset EndTime = call.EndTime == null ? conf.CreatedOn : (DateTimeOffset)call.EndTime;

                                                    CreateCommsLogAsync(call.Sid, "CONFERENCE", call.Status.ToString(), call.From, call.To, call.Direction, Price, answered_by, "USD",
                                                        0, answered_by, Duration, (DateTimeOffset)call.DateCreated, (DateTimeOffset)call.DateUpdated, StartTime, EndTime, CommsProvider: "TWILIO");

                                                    rcpnt.ConfJoined = StartTime;
                                                    rcpnt.ConfLeft = EndTime;
                                                    rcpnt.Status = call.Status.ToString().ToUpper();

                                                    //Get the recording of the call;
                                                    GetCallRecordingLog(call);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex; ;
                                        }
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetCallRecordingLog(CallResource call)
        {
            try
            {
                ResourceSet<RecordingResource> recordings = null;
                if (!SendInDirect)
                {
                    TwilioClient.Init(PHONESID, PHONETOKEN);
                    recordings = RecordingResource.Read(callSid: call.Sid);
                }
                else
                {
                    recordings = GetTwilioRecByApi(call.Sid);
                }

                if (recordings != null)
                {
                    foreach (var recording in recordings)
                    {

                        decimal Price = recording.Price == null ? 0M : Convert.ToDecimal(recording.Price);
                        int Duration = recording.Duration == null ? 0 : Convert.ToInt32(recording.Duration);

                        DateTimeOffset StartTime = call.StartTime == null ? (DateTimeOffset)SqlDateTime.MinValue.Value : (DateTimeOffset)call.StartTime;
                        DateTimeOffset EndTime = call.EndTime == null ? (DateTimeOffset)SqlDateTime.MinValue.Value : (DateTimeOffset)call.EndTime;

                        CreateCommsLogAsync(recording.Sid, "RECORDING", "COMPLETED", call.From, call.To, call.Direction, Price, "", "USD", 0, "",
                            Duration, (DateTimeOffset)recording.DateCreated, (DateTimeOffset)recording.DateUpdated, StartTime, EndTime, CommsProvider: "TWILIO");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateCommsLogAsync(string Sid, string CommType, string Status, string From, string To, string Direction, decimal Price, string AnsweredBy, string PriceUnit,
            int NumSegments, string Body, int Duration, DateTimeOffset DateCreated, DateTimeOffset DateUpdated, DateTimeOffset StartTime, DateTimeOffset EndTime,
            string ErrorCode = "", string ErrorMessage = "", string CommsProvider = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(Sid))
                {
                    var checksid = await db.Set<CommsLog>().Where(L=> L.Sid == Sid).FirstOrDefaultAsync();
                    if (checksid != null)
                    {
                        checksid.DateCreated = DBC.ToNullIfTooEarlyForDb(DateCreated);
                        checksid.Price = Price;
                        checksid.Status = Status;
                        checksid.Duration = Duration;
                        checksid.DateUpdated = DBC.ToNullIfTooEarlyForDb(DateUpdated);
                        checksid.EndTime = EndTime != null ? DBC.ToNullIfTooEarlyForDb(EndTime) : DateUpdated;
                        checksid.CommType = CommType;
                        checksid.NumSegments = NumSegments;
                        checksid.Body = Body;
                        checksid.DateSent = StartTime != null ? DBC.ToNullIfTooEarlyForDb(StartTime) : DateUpdated;
                        checksid.StartTime = EndTime != null ? DBC.ToNullIfTooEarlyForDb(EndTime) : DateUpdated;
                        checksid.ErrorCode = !string.IsNullOrEmpty(ErrorCode) ? ErrorCode : "";
                        checksid.ErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : "";
                        db.SaveChanges();
                    }
                    else
                    {
                        CommsLog CL = new CommsLog();
                        CL.Sid = Sid;
                        CL.DateCreated = DBC.ToNullIfTooEarlyForDb(DateCreated);
                        CL.DateUpdated = DBC.ToNullIfTooEarlyForDb(DateUpdated);
                        CL.DateSent = DBC.ToNullIfTooEarlyForDb(StartTime);
                        CL.Body = Body;
                        CL.NumSegments = NumSegments;
                        CL.ToFormatted = To;
                        CL.FromFormatted = From;
                        CL.Status = Status;
                        CL.StartTime = StartTime != null ? DBC.ToNullIfTooEarlyForDb(StartTime) : DateUpdated;
                        CL.EndTime = EndTime != null ? DBC.ToNullIfTooEarlyForDb(EndTime) : DateUpdated;
                        CL.Duration = Duration;
                        CL.Price = Price;
                        CL.PriceUnit = PriceUnit;
                        CL.Direction = Direction;
                        CL.AnsweredBy = AnsweredBy;
                        CL.ErrorCode = !string.IsNullOrEmpty(ErrorCode) ? ErrorCode : "";
                        CL.ErrorMessage = !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : "";
                        CL.CommType = CommType;
                        CL.CommsProvider = CommsProvider;
                        await db.AddAsync(CL);
                       await  db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void GetCommsPrice()
        {
            try
            {
                //Get all the countries where voice and sms are supported
                GetTwilioVoiceCountries();
                GetTwilioSMSCountries();

                //get the price information for all the countries
                DownloadTwilioPricing();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void DownloadTwilioPricing()
        {
            try
            {
                var countries =await  db.Set<Country>().Where(C=> C.PhoneAvailable == true || C.Smsavailable == true && C.CountryPhoneCode != null).ToListAsync();
                foreach (var country in countries)
                {
                    GetVoicePricing(country.Iso2code, country.CountryCode);
                    GetSMSPricing(country.Iso2code, country.CountryCode, country.CountryPhoneCode);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void GetVoicePricing(string ISO2Code, string CountryCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(ISO2Code))
                {

                    TwilioClient.Init(PHONESID, PHONETOKEN);

                    UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Getting voice prefix price information for country: " + CountryCode));
                    FireUpdateEvent(UEA);

                    var country = Twilio.Rest.Pricing.V1.Voice.CountryResource.Fetch(ISO2Code);

                    UpdateEventArgs UEA2 = new UpdateEventArgs(DBC.LogWrite("Total " + country.OutboundPrefixPrices.Count + " prefix found for country " + CountryCode));
                    FireUpdateEvent(UEA2);

                    foreach (var price in country.OutboundPrefixPrices)
                    {
                        foreach (var prefix in price.Prefixes)
                        {
                            var prfxrec =  db.Set<TwilioPricing>().Where(TPR=> TPR.CountryIso2 == ISO2Code && TPR.DesinationPrefix == prefix && TPR.ChannelType == "PHONE" ).FirstOrDefault();
                            if (prfxrec != null)
                            {
                                prfxrec.CurrentPrice = Convert.ToDecimal(price.CurrentPrice);
                                prfxrec.BasePrice = Convert.ToDecimal(price.BasePrice);
                                prfxrec.FriendlyName = price.FriendlyName;
                                prfxrec.UpdateTime = DateTimeOffset.Now;
                                db.SaveChanges();

                                UpdateEventArgs UEA3 = new UpdateEventArgs(DBC.LogWrite("Voice price information updated for country " + CountryCode + ": " + prefix));
                                FireUpdateEvent(UEA3);

                            }
                            else
                            {
                               await  AddTwilioPrice(ISO2Code, CountryCode, "PHONE", prefix, Convert.ToDecimal(price.BasePrice), Convert.ToDecimal(price.CurrentPrice),
                                    price.FriendlyName);

                                UpdateEventArgs UEA4 = new UpdateEventArgs(DBC.LogWrite("Voice price information added for country " + CountryCode + ": " + prefix));
                                FireUpdateEvent(UEA4);
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

        public async void GetSMSPricing(string ISO2Code, string CountryCode, string DialingCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(DialingCode) && !string.IsNullOrEmpty(ISO2Code))
                {
                    TwilioClient.Init(PHONESID, PHONETOKEN);
                    UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Getting voice prefix price information for country: " + CountryCode));
                    FireUpdateEvent(UEA);

                    var country = Twilio.Rest.Pricing.V1.Messaging.CountryResource.Fetch(ISO2Code);

                    UpdateEventArgs UEA2 = new UpdateEventArgs(DBC.LogWrite("Total " + country.OutboundSmsPrices.Count + " prefix found for country " + CountryCode));
                    FireUpdateEvent(UEA2);

                    foreach (var carrier in country.OutboundSmsPrices)
                    {
                        foreach (var price in carrier.Prices)
                        {
                            string numberType = price.NumberType.ToString();
                            var prfxrec =  db.Set<TwilioPricing>()
                                           .Where(TPR => TPR.CountryIso2 == ISO2Code && TPR.FriendlyName == carrier.Carrier
                                           && TPR.ChannelType == "SMS" && TPR.NumberType == numberType
                                           ).FirstOrDefault();
                            if (prfxrec != null)
                            {
                                prfxrec.CurrentPrice = Convert.ToDecimal(price.CurrentPrice);
                                prfxrec.BasePrice = Convert.ToDecimal(price.BasePrice);
                                prfxrec.FriendlyName = carrier.Carrier;
                                prfxrec.NumberType = price.NumberType.ToString();
                                prfxrec.UpdateTime = DateTimeOffset.Now;
                                db.SaveChanges();

                                int PriceID = prfxrec.Id;
                                UpdateEventArgs UEA3 = new UpdateEventArgs(DBC.LogWrite(PriceID + ": SMS price information updated for country " + CountryCode + ": " + DialingCode));
                                FireUpdateEvent(UEA3);

                            }
                            else
                            {
                                int PriceID =await  AddTwilioPrice(ISO2Code, CountryCode, "SMS", DialingCode, Convert.ToDecimal(price.BasePrice), Convert.ToDecimal(price.CurrentPrice),
                                    carrier.Carrier, price.NumberType.ToString());

                                UpdateEventArgs UEA4 = new UpdateEventArgs(DBC.LogWrite(PriceID + ": SMS price information added for country " + CountryCode + ": " + DialingCode));
                                FireUpdateEvent(UEA4);
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

        public async Task<int> AddTwilioPrice(string ISO2Code, string ISO3Code, string ChannelType, string Prefix, decimal BasePrice, decimal CurrentPrice, string FriendyName, string NumberType = "")
        {
            try
            {
                TwilioPricing TP = new TwilioPricing();
                TP.BasePrice = BasePrice;
                TP.ChannelType = ChannelType;
                TP.CountryCode = ISO3Code;
                TP.CountryIso2 = ISO2Code;
                TP.CurrentPrice = CurrentPrice;
                TP.DesinationPrefix = Prefix;
                TP.FriendlyName = FriendyName;
                TP.NumberType = NumberType;
                TP.UpdateTime = DateTimeOffset.Now;
                await db.AddAsync(TP);
                await db.SaveChangesAsync();
                return TP.Id;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }

        public void GetTwilioVoiceCountries()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                var countries = Twilio.Rest.Pricing.V1.Voice.CountryResource.Read(300);

                foreach (var country in countries)
                {
                    var cntr = db.Set<Country>().Where(C=> C.Iso2code == country.IsoCountry).FirstOrDefault();
                    if (cntr != null)
                    {
                        cntr.PhoneAvailable = true;
                        cntr.VoicePriceUrl = country.Url.ToString();

                        UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Country " + cntr.Name + " voice api url is updated"));
                        FireUpdateEvent(UEA);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void GetTwilioSMSCountries()
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);
                var countries = Twilio.Rest.Pricing.V1.Messaging.CountryResource.Read(500);
                foreach (var country in countries)
                {
                    var cntr =  db.Set<Country>().Where(C=> C.Iso2code == country.IsoCountry).FirstOrDefault();
                    if (cntr != null)
                    {
                        cntr.Smsavailable = true;
                        cntr.SmspriceUrl = country.Url.ToString();
                        UpdateEventArgs UEA = new UpdateEventArgs(DBC.LogWrite("Country " + cntr.Name + " SMS api url is updated"));
                        FireUpdateEvent(UEA);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async void UpdateBalance_SMS(string SMSSid)
        {
            try
            {
                var message = GetTwilioMessage(SMSSid);

                if (message != null)
                {

                    int tmpErrorCode = message.ErrorCode != null ? (int)message.ErrorCode : 0;
                    string ErrorCode = !string.IsNullOrEmpty(tmpErrorCode.ToString()) ? tmpErrorCode.ToString() : "NA";
                    string ErrorMessage = !string.IsNullOrEmpty(message.ErrorMessage) ? message.ErrorMessage : "NA";
                    string Price = message.Price != null ? message.Price : "0";

                   await CreateCommsLogAsync(message.Sid, "TEXT", message.Status.ToString(), message.From.ToString(), message.To, message.Direction.ToString(),
                        Convert.ToDecimal(Price), "self", message.PriceUnit, Convert.ToInt32(message.NumSegments), message.Body, 0, (DateTime)message.DateCreated,
                        (DateTime)message.DateUpdated, (DateTime)message.DateSent, (DateTime)message.DateSent, ErrorCode, ErrorMessage, CommsProvider: "TWILIO");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  async Task UpdateBalance_PHONE(string CallSid)
        {
            try
            {
                var call = GetTwilioCall(CallSid);

                if (call != null)
                {

                    decimal Price = call.Price == null ? 0 : Convert.ToDecimal(call.Price);
                    int Duration = call.Duration == null ? 0 : Convert.ToInt32(call.Duration);
                    string answered_by = call.AnsweredBy == null ? "" : call.AnsweredBy;

                    DateTime StartTime = call.StartTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)call.StartTime;
                    DateTime EndTime = call.EndTime == null ? (DateTime)SqlDateTime.MinValue : (DateTime)call.EndTime;

                    await CreateCommsLogAsync(call.Sid, "PHONE", call.Status.ToString(), call.From, call.To, call.Direction, Price, answered_by, "USD", 0, "", Duration,
                               (DateTime)call.DateCreated, (DateTime)call.DateUpdated, StartTime, EndTime, CommsProvider: "TWILIO");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TwilioPriceByNumber GetLocalPrice(string PhoneNumber, string Channel)
        {
            TwilioPriceByNumber tp = new TwilioPriceByNumber
            {
                Price = 0
            };
            try
            {
                var pPhoneNumber = new SqlParameter("@PhoneNumber", PhoneNumber);
                var pChannel = new SqlParameter("@Channel", Channel);
                
                    tp = db.Set<TwilioPriceByNumber>().FromSqlRaw("exec Get_Twilio_Pricing_By_Number @PhoneNumber, @Channel", pPhoneNumber, pChannel).FirstOrDefault();
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

        public void ClearTwilioLogs(int CompanyID)
        {
            try
            {
                TwilioClient.Init(PHONESID, PHONETOKEN);

                List<TwilioLogToClear> logs = GetTwilioLogsToClear(CompanyID);
                foreach (TwilioLogToClear log in logs)
                {
                    if (log.MethodName.ToUpper() == "TEXT")
                    {
                        try
                        {
                            MessageResource.Delete(log.Sid);
                        }
                        catch (Exception)
                        {
                        }

                    }
                    else if (log.MethodName.ToUpper() == "PHONE")
                    {
                        try
                        {
                            CallResource.Delete(log.Sid);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TwilioLogToClear> GetTwilioLogsToClear(int CompanyID)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
               
                    var result = db.Set<TwilioLogToClear>().FromSqlRaw("exec Get_Twilio_Logs_To_Clear @CompanyID", pCompanyID).ToList();
                    return result;
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}
