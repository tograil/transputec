using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Base;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace CrisesControl.Infrastructure.Repositories {
    public class CommunicationRepository : ICommunicationRepository {
        private readonly CrisesControlContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;
        private readonly DBCommon DBC;
        private readonly Messaging _MSG;
        private readonly SendEmail _SDE;
        private readonly ILogger<CommunicationRepository> _logger;

        private int UserID;
        private int CompanyID;
            
        public CommunicationRepository(CrisesControlContext context, HttpContextAccessor httpContextAccessor, Messaging MSG, SendEmail SDE, ILogger<CommunicationRepository> logger) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            DBC = new DBCommon(_context, _httpContextAccessor);
            _MSG = MSG;
            _SDE = SDE;
            _logger = logger;
        }

        public async Task<List<UserConferenceItem>> GetUserActiveConferences() {
            try {

                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await _context.Set<UserConferenceItem>().FromSqlRaw("exec Pro_Get_User_Active_Conference_List {0},{1}", pCompanyID, pUserId).ToListAsync();

                return result;
            } catch (Exception) {

                throw;
            }
        }
        public async Task<string> HandelCallResponse(string callSid, string callStatus, string from, string to, int duration = 0, string operato = "TWILIO")
        {
            try
            {
                string Message = "";
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join M in _context.Set<Message>() on MD.MessageId equals M.MessageId
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               where MD.CloudMessageId == callSid
                               select new { MD, M, ML, U }).FirstOrDefault();
                if (MsgRslt != null)
                {

                    string TimeZoneId = await DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;
                    int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("PHONE_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                    callStatus = callStatus.ToUpper() == "NOT-ANSWERED" ? "NO-ANSWER" : callStatus.ToUpper();

                    if (callStatus.ToUpper() == "COMPLETED")
                    {
                        dlvStatus = 1;
                        _context.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (callStatus.ToUpper() == "NO-ANSWER" || callStatus.ToUpper() == "NOT-ANSWERED" || callStatus.ToUpper() == "BUSY" || callStatus.ToUpper() == "FAILED")
                    {
                        dlvStatus = 2;

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                            
                            LogUndelivered(MsgRslt.M.MessageId, "PHONE", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }

                        await _context.SaveChangesAsync();
                    }
                    else if (callStatus.ToUpper() == "FAILED" || callStatus.ToUpper() == "CANCELED")
                    {
                        dlvStatus = 3;
                        await _context.SaveChangesAsync();
                    }

                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    MsgRslt.ML.DateDelivered = dtNow;
                    MsgRslt.ML.UpdatedOn = dtNow;

                    
                    MsgRslt.ML.MessageDelvStatus = dlvStatus;
                    MsgRslt.ML.UpdatedBy = MsgRslt.U.UserId;
                    MsgRslt.ML.UpdatedOn = dtNow;
                    _context.Update(MsgRslt);
                    await _context.SaveChangesAsync();

                    CommsLogsHelper CLH = new CommsLogsHelper(_context, _httpContextAccessor);
                    DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                    DateTimeOffset dateCreated = MsgRslt.ML.DateSent.UtcDateTime;
                    DateTimeOffset endCallTime = utcNow.AddSeconds(duration);
                    CLH.CreateCommsLogAsync(callSid, "PHONE", callStatus, from, to, "outbound-api", 0, "self", "USD", 1, "", duration, dateCreated, utcNow, utcNow, endCallTime, CommsProvider: operato);

                    if (operato.ToUpper() == "TWILIO")
                        CLH.DownloadAndCreateTwilioLog(callSid, "PHONE");

                    Message = "Delivery confirmation processed";
                    
                }
                else
                {
                   
                    Message = "Message record not found";
                    
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task LogUndelivered(int messageId, string method, int messageDeviceId, int attempt, int companyId, string timeZoneId)
        {
            DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
            try
            {
                UndeliveredMessage UM = new UndeliveredMessage();
                UM.Id = Guid.NewGuid();
                UM.Attempt = attempt;
                UM.MessageId = messageId;
                UM.DateCreated = DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                UM.MessageDeviceId = messageDeviceId;
                UM.MethodName = method;
                UM.ScheduleFlag = 0;
                UM.CompanyId = companyId;
               await _context.AddAsync(UM);
               await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> HandelUnifonicCallResponse(string referenceId, string callSId, string callStatus, string From, string To, double CallTimestamp, int Duration = 0, string Operator = "TWILIO")
        {
            try
            {
                string Message = "";
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join M in _context.Set<Message>() on MD.MessageId equals M.MessageId
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join MT in _context.Set<MessageTransaction>() on ML.MessageListId equals MT.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               where MT.CloudMessageId == referenceId && MT.DeviceAddress == To
                               select new { MD, M, ML, U, MT }).FirstOrDefault();
                if (MsgRslt != null)
                {

                    string TimeZoneId =await  DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;
                    int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("PHONE_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                    callStatus = callStatus.ToUpper() == CallStatus.NOTANSWERED.ToCString() ? CallStatus.NOANSWER.ToCString() : callStatus.ToUpper();

                    if (callStatus.ToUpper() == CallStatus.COMPLETED.ToCString())
                    {
                        dlvStatus = 1;
                        _context.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (callStatus.ToUpper() == CallStatus.NOANSWER.ToCString() || callStatus.ToUpper() == CallStatus.NOTANSWERED.ToCString() || callStatus.ToUpper() == CallStatus.BUSY.ToCString() || callStatus.ToUpper() == CallStatus.FAILED.ToCString())
                    {
                        dlvStatus = 2;

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                           await LogUndelivered(MsgRslt.M.MessageId, "PHONE", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }

                        await _context.SaveChangesAsync();
                    }
                    else if (callStatus.ToUpper() == CallStatus.FAILED.ToCString() || callStatus.ToUpper() == CallStatus.CANCELED.ToCString())
                    {
                        dlvStatus = 3;
                        await _context.SaveChangesAsync();
                    }

                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    MsgRslt.ML.DateDelivered = dtNow;
                    MsgRslt.ML.UpdatedOn = dtNow;

                    MsgRslt.ML.MessageDelvStatus = dlvStatus;
                    MsgRslt.ML.UpdatedBy = MsgRslt.U.UserId;
                    MsgRslt.ML.UpdatedOn = dtNow;
                    MsgRslt.MT.CloudMessageId = callSId;
                    _context.Update(MsgRslt);
                    await _context.SaveChangesAsync();

                    CommsLogsHelper CLH = new CommsLogsHelper(_context,_httpContextAccessor);
                    DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                    DateTimeOffset dateCreated = MsgRslt.MT.CreatedOn.Value;
                    DateTimeOffset endTime = DateTimeOffset.FromUnixTimeSeconds((long)CallTimestamp);
                    //DateTimeOffset endTime = CallTimestamp;
                    DateTimeOffset startTime = endTime.AddSeconds(Duration * -1);

                   await CLH.CreateCommsLogAsync(callSId, "PHONE", callStatus, From, To, "outbound-api", 0, "self", "USD", 1, "", Duration, dateCreated, utcNow,
                        startTime, endTime, CommsProvider: Operator);

                    
                    Message = "Delivery confirmation processed";
                    
                }
                else
                {
                    Message = "Message record not found";
                    
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> HandelSMSResponse(string messageSid, string smsStatus, string from, string to, string body, string operato = "TWILIO")
        {

            try
            {
                string Message = "";
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join M in _context.Set<Message>() on MD.MessageId equals M.MessageId
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               join MT in _context.Set<MessageTransaction>() on MD.CloudMessageId equals MT.CloudMessageId
                               where MD.CloudMessageId == messageSid && MT.CloudMessageId == messageSid
                               select new { MD, M, ML, U, MT.CreatedOn }).FirstOrDefault();

                if (MsgRslt != null)
                {

                    string TimeZoneId =await DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;

                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    MsgRslt.ML.DateDelivered = dtNow;
                    MsgRslt.ML.UpdatedOn = dtNow;

                    if (smsStatus.ToUpper() == SmsStatus.DELIVERED.ToSString() || smsStatus.ToUpper() == SmsStatus.SENT.ToSString() || smsStatus.ToUpper() == SmsStatus.ACCEPTED.ToSString() || smsStatus == "2")
                    {
                        dlvStatus = 1;
                        //db.MessageDevice.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (smsStatus.ToUpper() == SmsStatus.UNDELIVERED.ToSString() || smsStatus.ToUpper() == SmsStatus.REJECTED.ToSString() || smsStatus == "1")
                    {
                        dlvStatus = 2;
                        int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("TEXT_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                          await LogUndelivered(MsgRslt.M.MessageId, "TEXT", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }
                    }
                    else if (smsStatus.ToUpper() == SmsStatus.FAILED.ToSString() || smsStatus == "3")
                    {
                        dlvStatus = 3;
                    }

                    //var translog = (from MT in db.MessageTransaction where MT.CloudMessageId == MessageSid select MT).FirstOrDefault();
                    //if(translog != null) {
                    //    translog.LogCollected = false;
                    //}

                    MsgRslt.ML.MessageDelvStatus = dlvStatus;
                    MsgRslt.ML.UpdatedBy = MsgRslt.U.UserId;
                    MsgRslt.ML.UpdatedOn = dtNow;
                    _context.Update(MsgRslt);
                    await _context.SaveChangesAsync();

                    CommsLogsHelper CLH = new CommsLogsHelper(_context,_httpContextAccessor);
                    DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                    DateTimeOffset dateCreated = MsgRslt.ML.DateSent.UtcDateTime;
                    await CLH.CreateCommsLogAsync(messageSid, "TEXT", smsStatus, from, to, "outbound-api", 0, "self", "USD", 1, body, 0, dateCreated, utcNow, utcNow, utcNow, CommsProvider: operato);

                    if (operato == "CM")
                    {
                        CLH.DownloadAndCreateCMSMSLog(messageSid, (DateTimeOffset)MsgRslt.CreatedOn);
                    }
                    else if (operato == "TWILIO")
                    {
                        CLH.DownloadAndCreateTwilioLog(messageSid, "TEXT");
                    }

                    Message = "Delivery confirmation processed";
                }
                else
                {
                    
                    Message = "Message record not found";

                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> HandelTwoFactor(string messageSid, SmsStatus smsStatus)
        {

            try
            {
                string Message = "";
                var tfrec = await _context.Set<TwoFactorAuthLog>()
                             .Where(TF=> TF.CloudMessageId == messageSid).FirstOrDefaultAsync();

                if (tfrec != null)
                {
                    tfrec.Status = smsStatus.ToSString();
                    await _context.SaveChangesAsync();

                    
                    Message = "Delivery confirmation processed";
                }
                else
                {
                    Message = "Message record not found";

                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string > TwilioCallAck(HttpContext context)
        {
            try
            {
                string emailmessage = "There was an error while processing the delivery confirmation";
                emailmessage = "application/xml";
                string CloudMessageId = context.Request.Form["CallSid"].ToArray() == null ? "" : context.Request.Form["CallSid"];
                string Digits = context.Request.Form["Digits"].ToArray() == null ? "" : context.Request.Form["Digits"];
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               where MD.CloudMessageId == CloudMessageId
                               select new { MD, ML, U }).FirstOrDefault();
                if (MsgRslt != null)
                {

                    string TimeZoneId =await DBC.GetTimeZoneVal(MsgRslt.U.UserId);

                    //DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                    if (MsgRslt.ML.MessageAckStatus == 0)
                    {
                        //MsgRslt.ML.UpdatedOn = dtNow;
                        //MsgRslt.ML.UserLocationLat = "0";
                        //MsgRslt.ML.UserLocationLong = "0";
                        if (!string.IsNullOrEmpty(Digits))
                        {
                            if (Digits == "1")
                            {
                                //MsgRslt.ML.MessageAckStatus = 1;
                                //MsgRslt.ML.DateAcknowledge = dtNow;
                                Messaging MSG = new Messaging(_context,_httpContextAccessor,DBC);
                                MSG.AcknowledgeMessage(MsgRslt.U.UserId, MsgRslt.ML.MessageId, MsgRslt.ML.MessageListId, "0", "0", "PHONE", 0, TimeZoneId);
                            }
                        }
                        //MsgRslt.ML.UpdatedBy = MsgRslt.U.UserId;
                        _context.Remove(MsgRslt.MD);
                    }

                 

                    await _context.SaveChangesAsync();

                    emailmessage="<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Say voice=\"woman\">Thank you for the acknowledement. Goodbye</Say></Response>";

                }
                else
                {
                    emailmessage="<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Say voice=\"woman\">Thank you for the acknowledement. Goodbye</Say></Response>";
                }
                return emailmessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> HandelPushResponse(int sendBackId)
        {

            try
            {
                string Message = string.Empty;
                var MsgRslt = await  _context.Set<MessageDevice>().Include(M=> M.MessageList.Message).Include(ML=>ML.MessageList).Include(U=>U.MessageList.User)
                                    .Where(MD=> MD.MessageDeviceId == sendBackId).FirstOrDefaultAsync();

                if (MsgRslt != null)
                {
                    string TimeZoneId =await  DBC.GetTimeZoneVal(MsgRslt.MessageList.User.UserId);

                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    MsgRslt.MessageList.DateDelivered = dtNow;
                    MsgRslt.MessageList.UpdatedOn = dtNow;
                    MsgRslt.MessageList.MessageDelvStatus = 1;
                    MsgRslt.MessageList.UpdatedBy = MsgRslt.MessageList.User.UserId;
                    MsgRslt.MessageList.UpdatedOn = dtNow;
                    _context.Update(MsgRslt);
                    await _context.SaveChangesAsync();

                    Message = "Delivery confirmation processed";
                }
                else
                {
                  Message = "Message record not found";
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> HandelConfResponse(string callSid, string conferenceSid, string statusCallbackEvent)
        {

            try
            {
                string Message = string.Empty;
                if (statusCallbackEvent == "participant-join" || statusCallbackEvent == "participant-leave")
                {
                    var getConf = (from CD in _context.Set<ConferenceCallLogDetail>()
                                   join CH in _context.Set<ConferenceCallLogHeader>() on CD.ConferenceCallId equals CH.ConferenceCallId
                                   where CD.SuccessCallId == callSid
                                   select new { CD, CH }).FirstOrDefault();
                    if (getConf != null)
                    {

                        string TimeZoneId =await DBC.GetTimeZoneVal(getConf.CD.UserId);
                        DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                        int userconfcount =await  _context.Set<ConferenceCallLogDetail>().Where(w => w.UserId == getConf.CD.UserId && w.ConferenceCallId == getConf.CH.ConferenceCallId).CountAsync();

                        if (getConf.CH.InitiatedBy == getConf.CD.UserId && userconfcount == 1 &&
                            (statusCallbackEvent == "conference-start" || statusCallbackEvent == "participant-join"))
                        {
                            try
                            {
                               await CallConfParticipants(getConf.CH.ConferenceCallId);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                        getConf.CH.CloudConfId = conferenceSid;
                        if (statusCallbackEvent == "participant-join")
                        {
                            getConf.CD.Status = "JOINED";
                            getConf.CD.ConfJoined = dtNow;

                            if (getConf.CH.CurrentStatus == "CREATED")
                            {
                                getConf.CH.CurrentStatus = "IN-PROGRESS";
                                getConf.CH.ConfrenceStart = DateTimeOffset.Now;
                            }
                        }
                        else if (statusCallbackEvent == "participant-leave")
                        {
                            if (getConf.CD.ConfJoined.Year > 2000)
                            {
                                getConf.CD.Status = "LEFT";
                                getConf.CD.ConfLeft = dtNow;
                            }

                            bool live_prcpnt = await  (from CD in _context.Set<ConferenceCallLogDetail>()
                                                join CH in _context.Set<ConferenceCallLogHeader>() on CD.ConferenceCallId equals CH.ConferenceCallId
                                                where CH.CloudConfId == conferenceSid &&
                                                (CD.Status == "JOINED" || CD.Status == "IN-PROGRESS" || CD.Status == "RINGING" || CD.Status == "INITIATED")
                                                select CD).AnyAsync();
                            if (!live_prcpnt)
                            {
                               await  EndConferenceCall(conferenceSid);
                            }
                        }
                        await _context.SaveChangesAsync();
                        Message = "Conferance details saved";

                    }
                    else
                    {
                        Message = "Message record not found";
                    }
                    return Message;
                }
                else if (statusCallbackEvent == "conference-start" || statusCallbackEvent == "conference-end")
                {
                    var getConf = await _context.Set<ConferenceCallLogHeader>()
                                   .Where(CH=> CH.CloudConfId == conferenceSid).FirstOrDefaultAsync();
                    if (getConf != null)
                    {
                        string TimeZoneId =await DBC.GetTimeZoneVal(getConf.CreatedBy);
                        DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                        getConf.CloudConfId = conferenceSid;

                        if (statusCallbackEvent == "conference-start")
                        {
                            getConf.CurrentStatus = "STARTED";
                            getConf.ConfrenceStart = dtNow;
                        }
                        else if (statusCallbackEvent == "conference-end")
                        {
                            if (getConf.ConfrenceStart.Year > 2000)
                            {
                                getConf.ConfrenceEnd = dtNow;
                                getConf.CurrentStatus = "ENDED";
                            }
                        }
                       await _context.SaveChangesAsync();
                        Message = "Conferance details saved";
                    }
                    else
                    {
                        Message = "Message record not found";
                    }
                }
                else
                {
                    Message = "Message record not found";
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> EndConferenceCall(string conferenceId)
        {
            try
            {
                var conf = await  _context.Set<ConferenceCallLogHeader>().Where(C=> C.CloudConfId == conferenceId).FirstOrDefaultAsync();
                if (conf != null)
                {
                    CommsHelper CH = new CommsHelper(_context,_httpContextAccessor);
                    bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                    string DataCenter = DBC.GetCompanyParameter("TWILIO_EDGE_LOCATION", conf.CompanyId);

                    dynamic CommsAPI = CH.InitComms("TWILIO", dataCenter: DataCenter);
                    CommsAPI.SendInDirect = SendInDirect;
                    conf.CurrentStatus = "COMPLETED";
                    await _context.SaveChangesAsync();
                    return CommsAPI.EndConferenceCall(conferenceId);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> CallConfParticipants(int confCallId, string source = "APP")
        {
            try
            {

                //Initialize the parmas
                //string ClientId = string.Empty;
                //string ClientSecret = string.Empty;
                string FromNumber = string.Empty;
                string APIClass = string.Empty;
                string CallBackUrl = string.Empty;
                string MessageXML = string.Empty;
                string Message = string.Empty;
                var confHead = await  _context.Set<ConferenceCallLogHeader>().Where(CH=> CH.ConferenceCallId == confCallId).FirstOrDefaultAsync();
                if (confHead != null)
                {

                    if (confHead.CurrentStatus.ToUpper() == "COMPLETED")
                    {
                        
                        Message = "The conference call has been ended.";
                        return Message;
                    }

                    //Get Participant list of conference
                    //Use: EXEC [dbo].[Pro_Communication_GetConferenceParticipants] @ConfCallId
                    var conf_pc = await _context.Set<ConferenceCallLogDetail>()
                                   .Where(CD=> CD.ConferenceCallId == confCallId).ToListAsync();

                    if (UserID > 0)
                    {
                        conf_pc = conf_pc.Where(w => w.UserId == UserID).Take(1).ToList();
                    }

                    if ((conf_pc.Count > 0 && UserID <= 0) || (UserID > 0 && conf_pc.Count == 1))
                    {
                        //Get conference header


                        //Get the selected conferance api for the company and set the requrest api params.
                        //Use: EXEC [dbo].[Pro_Global_GetSystemParameter] @ParamNames for LookupWithKey
                        //Use: EXEC [dbo].[Pro_Global_GetCompanyParameter] @CompanyID,@ParamNames for GetCompanyParameter
                        string CONF_API = DBC.GetCompanyParameter("CONFERANCE_API", CompanyID);
                        string DataCenter = DBC.GetCompanyParameter("TWILIO_EDGE_LOCATION", CompanyID);

                        string RetryNumberList = DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", CompanyID, FromNumber);
                        List<string> FromNumberList = RetryNumberList.Split(',').ToList();
                        FromNumber = FromNumberList.FirstOrDefault();

                        //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                        CallBackUrl = DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                        MessageXML = DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");
                        bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                        string TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

                        string ConfWelcome = DBC.LookupWithKey("TWILIO_DEFAULT_CONF_MSG");

                        //Check if conference is started for an Incident
                        if (confHead.ActiveIncidentId > 0)
                        {
                            var InciDtl =await   _context.Set<IncidentActivation>()
                                               //join I in db.Incident on IA.IncidentId equals I.IncidentId
                                           .Where(IA=> IA.IncidentActivationId == confHead.ActiveIncidentId)
                                           .Select(IA=> IA.Name).FirstOrDefaultAsync();

                            if (InciDtl != null)
                            {
                                string CONF_MAIN_MESSAGE = DBC.LookupWithKey("INCIDENT_CONF_MAIN_MESSAGE");
                                ConfWelcome = CONF_MAIN_MESSAGE.Replace("{INCIDENTNAME}", InciDtl.ToString());
                            }
                        }

                        //Create Instenace of the Comms Api choosen by customer
                        CommsHelper CMH = new CommsHelper( _context, _httpContextAccessor);
                        dynamic CommsAPI = CMH.InitComms(CONF_API, dataCenter: DataCenter);
                        CommsAPI.IsConf = confHead.Record;
                        CommsAPI.ConfRoom = confHead.ConfRoomName;
                        CommsAPI.SendInDirect = SendInDirect;
                        CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                        Messaging MSG = new Messaging(_context,_httpContextAccessor,DBC);

                        string CallId = string.Empty;
                        string CalledOn = "MOBILE";
                        string Status = string.Empty;
                        string TimeZoneId = DBC.GetTimeZoneByCompany(CompanyID);

                        //Calling each participant except the moderator.
                        foreach (var pc in conf_pc)
                        {
                            if (!string.IsNullOrEmpty(pc.PhoneNumber) && (pc.UserId != confHead.InitiatedBy || (UserID > 0)))
                            {
                                CMH.MakeConferenceCall(FromNumber, CallBackUrl, MessageXML, CommsAPI, out CallId, pc.PhoneNumber, pc.Landline, out Status, CalledOn);

                                if (UserID > 0)
                                {
                                   await MSG.CreateConferenceDetail("ADD", confCallId, UserID, pc.PhoneNumber, pc.Landline, CallId.ToString(), Status, TimeZoneId, 0, CalledOn);
                                }
                                else
                                {
                                  await  MSG.CreateConferenceDetail("UPDATE", 0, 0, "", "", CallId.ToString(), Status, "", pc.ConferenceCallDetailId, CalledOn);
                                }
                            }
                        }
                        if (source == "APP" && UserID > 0)
                        {
                            Message = "Conference call initiated.";
                            return Message;
                        }
                        return Message;
                    }
                    else
                    {
                        if (UserID > 0)
                        {
                            Message = "User was not a participant of the conference";
                            return Message;
                        }
                    }
                }
                else
                {
                   Message = "No conference found";
                    return Message;
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<string> HandelConfRecording(string conferenceSid, string recordingSid, string recordingUrl, string recordingStatus,
            int recordingFileSize, int duration)
        {

            try
            {
                string Message = string.Empty;
                var getConf =await  _context.Set<ConferenceCallLogHeader>()
                               .Where(CD=> CD.CloudConfId == conferenceSid).FirstOrDefaultAsync();

                if (getConf != null)
                {

                    string TimeZoneId =await DBC.GetTimeZoneVal(getConf.CreatedBy);
                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                    if (recordingStatus == "completed")
                    {
                        getConf.RecordingUrl = recordingUrl;
                        getConf.RecordingFileSize = recordingFileSize;
                        getConf.RecordingSid = recordingSid;
                        getConf.Duration = duration;
                        getConf.Record = true;
                        await _context.SaveChangesAsync();

                        Messaging MSG = new Messaging(_context,_httpContextAccessor,DBC);
                        MSG.DownloadRecording(recordingSid, getConf.CompanyId, recordingUrl);
                    }
                    Message = "Conferance details saved";
                    return Message;
                }
                else
                {
                    Message = "Message record not found";
                }
                return Message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int ActiveIncidentId = 0, int MessageId = 0, string ObjectType = "Incident")
        {
            
           

            //Initiatize the variables
            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string FromNumber = string.Empty;
            string APIClass = string.Empty;
            string CallBackUrl = string.Empty;
            string MessageXML = string.Empty;
            string TwilioRoutingApi = string.Empty;

            try
            {

                //Get the selected conferance api for the company and set the requrest api params.
                string CONF_API = DBC.GetCompanyParameter("CONFERANCE_API", companyId);
                string DataCenter = DBC.GetCompanyParameter("TWILIO_EDGE_LOCATION", companyId);

                bool RecordConf = Convert.ToBoolean(DBC.GetCompanyParameter("RECORD_CONFERENCE", companyId));
                bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

                //Create instance of CommsApi choosen by company
                dynamic CommsAPI = this.InitComms(CONF_API, dataCenter: DataCenter);

                CommsAPI.IsConf = true;
                CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                //Get API configraiton from sysparameters
                string RetryNumberList = DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", companyId, FromNumber);
                List<string> FromNumberList = RetryNumberList.Split(',').ToList();

                FromNumber = FromNumberList.FirstOrDefault();
                CallBackUrl = DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                MessageXML = DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

                //Get the user list to fetch their mobile numbers
                List<int> nUList = new List<int>();
                nUList.Add(userId);

                foreach (User cUser in userList)
                {
                    nUList.Add(cUser.UserId);
                }

                var tmpUserList = await  _context.Set<User>().
                                   Where(U=> U.CompanyId == companyId && nUList.Contains(U.UserId) && U.Status == 1)
                                   .Select(U=> new { UserId = U.UserId, ISD = U.Isdcode, PhoneNumber = U.MobileNo, U.Llisdcode, U.Landline }).Distinct().ToListAsync();

                

                //Create conference header
                int ConfHeaderId = _MSG.CreateConferenceHeader(companyId, userId, timeZoneId, CommsAPI.ConfRoom, RecordConf, ActiveIncidentId, MessageId, ObjectType);

                string CallId = string.Empty;
                int ModeratorCDId = 0;
                string ModeratorNumber = string.Empty;
                string ModeratorLandline = string.Empty;
                string Status = string.Empty;

                //Loop through with each user and their phone number to make the call
                foreach (var uItem in tmpUserList)
                {
                    string Mobile = DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);
                    string Landline = DBC.FormatMobile(uItem.Llisdcode, uItem.Landline);

                    if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                    {
                        int CallDetaildId = await _MSG.CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, Mobile, Landline, CallId.ToString(), "ADDED", timeZoneId, 0);
                        if (uItem.UserId == userId)
                        {
                            ModeratorCDId = CallDetaildId;
                            ModeratorNumber = Mobile;

                            if (!string.IsNullOrEmpty(Landline))
                                ModeratorLandline = Landline;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ModeratorNumber))
                {
                    string CallStatus = string.Empty;
                    string CalledOn = "MOBILE";

                    CallStatus = MakeConferenceCall(FromNumber, CallBackUrl, MessageXML, CommsAPI,  CallId, ModeratorNumber, ModeratorLandline,  Status, CalledOn);
                    await  _MSG.CreateConferenceDetail("UPDATE", 0, 0, "", "", CallId.ToString(), CallStatus, "", ModeratorCDId, CalledOn);
                }
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public async Task<string> MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI,  string callId,
            string mobileNumber, string landLineNumber, string status, string calledOn)
        {
            calledOn = "MOBILE";
            string CallStatus = string.Empty;
            
            //Initiate the call to the modrator
            Task<dynamic> calltask =await Task.Factory.StartNew(() => commsAPI.Call(fromNumber, mobileNumber, messageXML, callBackUrl));
            CommsStatus callrslt = calltask.Result;
            callId = callrslt.CommsId;

            if (!callrslt.Status)
            {
                status = "";
                CallStatus = callId = DBC.Left(callId, 50);
                if (!string.IsNullOrEmpty(landLineNumber))
                {
                    Task<dynamic> recalltask = Task.Factory.StartNew(() => commsAPI.Call(fromNumber, landLineNumber, messageXML, callBackUrl));
                    CommsStatus recallrslt = recalltask.Result;
                    if (recallrslt.Status)
                    {
                        calledOn = "LANDLINE";
                        CallStatus = recallrslt.CurrentAction;
                        status = recallrslt.CurrentAction;
                    }
                    else
                    {
                        callId = recallrslt.CommsId;
                        status = "";
                        CallStatus = callId = DBC.Left(callId, 50);
                    }
                }
            }
            else
            {
                status = callrslt.CurrentAction;
                CallStatus = callrslt.CurrentAction;
            }
            return CallStatus;
        }
        public async Task<dynamic> InitComms(string api_CLASS, string aPIClass = "", string clientId = "", string clientSecret = "", string dataCenter = "dublin")
        {
            
            try
            {

                int RetryCount = 2;
                int.TryParse(DBC.LookupWithKey(api_CLASS + "_MESSAGE_RETRY_COUNT"), out RetryCount);

                if (string.IsNullOrEmpty(aPIClass))
                    aPIClass = DBC.LookupWithKey(api_CLASS + "_API_CLASS");

                if (string.IsNullOrEmpty(clientId))
                    clientId = DBC.LookupWithKey(api_CLASS + "_CLIENTID");

                if (string.IsNullOrEmpty(clientSecret))
                    clientSecret = DBC.LookupWithKey(api_CLASS + "_CLIENT_SECRET");

                string[] TmpClass = aPIClass.Trim().Split('|');

                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

                Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
                Type type = assembly.GetType(TmpClass[1]);
                dynamic CommsAPI = Activator.CreateInstance(type);

                CommsAPI.ClientId = clientId;
                CommsAPI.Secret = clientSecret;
                CommsAPI.RetryCount = RetryCount;

                if (!string.IsNullOrEmpty(dataCenter))
                    CommsAPI.DataCenter = dataCenter;

                return CommsAPI;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<string> RejoinConference(int confCallId, int companyID, int userID, string source = "APP")
        {
            try
            {
                var result = CallConfParticipants(confCallId, source);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CommsStatus> TwilioText(TwilioRequest ip)
        {
            try
            {
                
                CommsHelper CH = new CommsHelper(_context, _httpContextAccessor);

                dynamic CommsAPI = CH.InitComms("TWILIO", "", ip.ClientId, ip.Secret, ip.DataCenter);

                CommsAPI.IsConf = false;
                CommsAPI.CommsDebug = ip.CommsDebug;
                CommsAPI.USE_MESSAGING_COPILOT = ip.UseMessagingCopilot;
                CommsAPI.MESSAGING_COPILOT_SID = ip.MessagingCopilotId;
                CommsAPI.SendInDirect = false;

                CommsStatus textrslt = new CommsStatus();

                if (ip.CommsDebug)
                {
                    System.Threading.Thread.Sleep(100);
                    textrslt.CommsId = "SMS" + Guid.NewGuid().ToString().Trim('-');
                    textrslt.CurrentAction = "QUEUED";
                    textrslt.Status = true;
                }
                else
                {
                    textrslt = CommsAPI.Text(ip.FromNumber, ip.DvcAddress, ip.MessageBody, ip.Callback);
                }
                return textrslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CommsStatus> TwilioCall(TwilioRequest ip)
        {
            try
            {
                CommsHelper CH = new CommsHelper(_context, _httpContextAccessor);

                CommsStatus callrslt = new CommsStatus();

                dynamic CommsAPI = CH.InitComms("TWILIO", "", ip.ClientId, ip.Secret, ip.DataCenter);

                CommsAPI.IsConf = false;
                CommsAPI.CommsDebug = ip.CommsDebug;
                CommsAPI.SendInDirect = false;

                if (ip.CommsDebug)
                {
                    System.Threading.Thread.Sleep(100);
                    callrslt.CommsId = "CA" + Guid.NewGuid().ToString().Trim('-');
                    callrslt.CurrentAction = "QUEUED";
                    callrslt.Status = true;
                }
                else
                {
                    callrslt = CommsAPI.Call(ip.FromNumber, ip.DvcAddress, ip.MessageXML, ip.Callback, ip.Record);
                }

                return callrslt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task DeleteRecording(string recordingId)
        {
        
            try
            {
                bool SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                CommsAPI.DeleteRecording(recordingId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MessageResource> TwilioTextLog(TwilioRequest ip)
        {
            MessageResource message = null;

            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);
                TwilioClient.SetEdge(ip.DataCenter);

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        message = await MessageResource.FetchAsync(ip.Sid);
                        if (message != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,  "Retry=" + i);
                    }
                }
                return message;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CallResource> TwilioCallLog(TwilioRequest ip)
        {
            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);
                TwilioClient.SetEdge(ip.DataCenter);

                CallResource call = null;

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        call =await CallResource.FetchAsync(ip.Sid);
                        if (call != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CommsLogs:FetchCallLog", "Retry=" + i);
                    }
                }

                return call;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ConferenceResource> TwilioConfLog(TwilioRequest ip)
        {
            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);
                TwilioClient.SetEdge(ip.DataCenter);

                ConferenceResource conference = null;

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        conference = await ConferenceResource.FetchAsync(ip.Sid);
                        if (conference != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CommsLogs:FetchConfLog", "Retry=" + i);
                    }
                }

                return conference;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResourceSet<RecordingResource>> TwilioRecordingLog(TwilioRequest ip)
        {
            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);
                TwilioClient.SetEdge(ip.DataCenter);

                ResourceSet<RecordingResource> rec = null;

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        rec =await RecordingResource.ReadAsync(callSid: ip.Sid);
                        if (rec != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CommsLogs:FetchRecLog", "Retry=" + i);
                    }
                }

                return rec;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VerificationResource> TwilioVerify(TwilioRequest ip)
        {
            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);

                CreateVerificationOptions option = new CreateVerificationOptions(ip.VerifyServiceID, ip.DvcAddress, ip.Method);
                VerificationResource message = null;

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        message =await  VerificationResource.CreateAsync(option);
                        if (message != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, "CommsClient:TwilioVerify", "Text: Retry=" + i);
                    }
                }
                return message;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VerificationCheckResource> TwilioVerifyCheck(TwilioRequest ip)
        {
            try
            {
                TwilioClient.Init(ip.ClientId, ip.Secret);

                VerificationCheckResource message = null;

                for (int i = 0; i < ip.RetryCount; i++)
                {
                    try
                    {
                        message = await  VerificationCheckResource.CreateAsync(
                            to: ip.DvcAddress,
                            code: ip.Code,
                            pathServiceSid: ip.VerifyServiceID
                        );
                        if (message != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, "CommsClient", "Text: Retry=" + i);
                    }
                }

                return message;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<dynamic> TwilioEndConferenceCall(TwilioRequest ip)
        {
            try
            {

                CommsHelper CH = new CommsHelper(_context,_httpContextAccessor);
                dynamic CommsAPI = CH.InitComms("TWILIO", "", ip.ClientId, ip.Secret, ip.DataCenter);
                CommsAPI.SendInDirect = false;
                var result = CommsAPI.EndConferenceCall(ip.Sid);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<HttpResponseMessage> DownloadRecording(string fileName)
        {
            try
            {

                string TwilioURL = DBC.LookupWithKey("RECORDING_URL");

                WebClient client = new WebClient();
                try
                {
                    MemoryStream stream = new MemoryStream(client.DownloadData(TwilioURL + fileName + ".mp3"), 0, 0, true, true);
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(stream.GetBuffer())
                    };
                    //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                    //    FileName = FileName
                    //};
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
