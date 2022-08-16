using AutoMapper;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Communication;
using CrisesControl.Core.Communication.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class CommunicationRepository : ICommunicationRepository {
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBCommon DBC;

        private int UserID;
        private int CompanyID;
            
        public CommunicationRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            DBC = new DBCommon(_context, _httpContextAccessor);
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
        public async Task<string> HandelCallResponse(string CallSid, string CallStatus, string From, string To, int Duration = 0, string Operator = "TWILIO")
        {
            try
            {
                string Message = "";
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join M in _context.Set<Message>() on MD.MessageId equals M.MessageId
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               where MD.CloudMessageId == CallSid
                               select new { MD, M, ML, U }).FirstOrDefault();
                if (MsgRslt != null)
                {

                    string TimeZoneId = DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;
                    int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("PHONE_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                    CallStatus = CallStatus.ToUpper() == "NOT-ANSWERED" ? "NO-ANSWER" : CallStatus.ToUpper();

                    if (CallStatus.ToUpper() == "COMPLETED")
                    {
                        dlvStatus = 1;
                        _context.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (CallStatus.ToUpper() == "NO-ANSWER" || CallStatus.ToUpper() == "NOT-ANSWERED" || CallStatus.ToUpper() == "BUSY" || CallStatus.ToUpper() == "FAILED")
                    {
                        dlvStatus = 2;

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                            
                            LogUndelivered(MsgRslt.M.MessageId, "PHONE", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }

                        await _context.SaveChangesAsync();
                    }
                    else if (CallStatus.ToUpper() == "FAILED" || CallStatus.ToUpper() == "CANCELED")
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
                    DateTimeOffset endCallTime = utcNow.AddSeconds(Duration);
                    CLH.CreateCommsLog(CallSid, "PHONE", CallStatus, From, To, "outbound-api", 0, "self", "USD", 1, "", Duration, dateCreated, utcNow, utcNow, endCallTime, CommsProvider: Operator);

                    if (Operator.ToUpper() == "TWILIO")
                        CLH.DownloadAndCreateTwilioLog(CallSid, "PHONE");

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
        public void LogUndelivered(int MessageID, string Method, int MessageDeviceID, int Attempt, int CompanyID, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
            try
            {
                UndeliveredMessage UM = new UndeliveredMessage();
                UM.Id = Guid.NewGuid();
                UM.Attempt = Attempt;
                UM.MessageId = MessageID;
                UM.DateCreated = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                UM.MessageDeviceId = MessageDeviceID;
                UM.MethodName = Method;
                UM.ScheduleFlag = 0;
                UM.CompanyId = CompanyID;
                _context.Add(UM);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> HandelUnifonicCallResponse(string referenceId, string callSId, string CallStatus, string From, string To, double CallTimestamp, int Duration = 0, string Operator = "TWILIO")
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

                    string TimeZoneId = DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;
                    int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("PHONE_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                    CallStatus = CallStatus.ToUpper() == "NOT-ANSWERED" ? "NO-ANSWER" : CallStatus.ToUpper();

                    if (CallStatus.ToUpper() == "COMPLETED")
                    {
                        dlvStatus = 1;
                        _context.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (CallStatus.ToUpper() == "NO-ANSWER" || CallStatus.ToUpper() == "NOT-ANSWERED" || CallStatus.ToUpper() == "BUSY" || CallStatus.ToUpper() == "FAILED")
                    {
                        dlvStatus = 2;

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                            LogUndelivered(MsgRslt.M.MessageId, "PHONE", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }

                        await _context.SaveChangesAsync();
                    }
                    else if (CallStatus.ToUpper() == "FAILED" || CallStatus.ToUpper() == "CANCELED")
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

                    CLH.CreateCommsLog(callSId, "PHONE", CallStatus, From, To, "outbound-api", 0, "self", "USD", 1, "", Duration, dateCreated, utcNow,
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

        public async Task<string> HandelSMSResponse(string MessageSid, string SmsStatus, string From, string To, string Body, string Operator = "TWILIO")
        {

            try
            {
                string Message = "";
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join M in _context.Set<Message>() on MD.MessageId equals M.MessageId
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               join MT in _context.Set<MessageTransaction>() on MD.CloudMessageId equals MT.CloudMessageId
                               where MD.CloudMessageId == MessageSid && MT.CloudMessageId == MessageSid
                               select new { MD, M, ML, U, MT.CreatedOn }).FirstOrDefault();

                if (MsgRslt != null)
                {

                    string TimeZoneId = DBC.GetTimeZoneVal(MsgRslt.U.UserId);
                    int dlvStatus = 0;

                    DateTimeOffset dtNow = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                    MsgRslt.ML.DateDelivered = dtNow;
                    MsgRslt.ML.UpdatedOn = dtNow;

                    if (SmsStatus.ToUpper() == "DELIVERED" || SmsStatus.ToUpper() == "SENT" || SmsStatus.ToUpper() == "ACCEPTED" || SmsStatus == "2")
                    {
                        dlvStatus = 1;
                        //db.MessageDevice.Remove(MsgRslt.MD);
                        await _context.SaveChangesAsync();
                    }
                    else if (SmsStatus.ToUpper() == "UNDELIVERED" || SmsStatus.ToUpper() == "REJECTED" || SmsStatus == "1")
                    {
                        dlvStatus = 2;
                        int MaxAttempt = Convert.ToInt32(DBC.GetCompanyParameter("TEXT_MAX_ATTEMPT", MsgRslt.U.CompanyId));

                        if (MsgRslt.MD.Attempt <= MaxAttempt && MaxAttempt != 0)
                        {
                           LogUndelivered(MsgRslt.M.MessageId, "TEXT", MsgRslt.MD.MessageDeviceId, MsgRslt.MD.Attempt, MsgRslt.U.CompanyId, TimeZoneId);
                        }
                    }
                    else if (SmsStatus.ToUpper() == "FAILED" || SmsStatus == "3")
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
                    CLH.CreateCommsLog(MessageSid, "TEXT", SmsStatus, From, To, "outbound-api", 0, "self", "USD", 1, Body, 0, dateCreated, utcNow, utcNow, utcNow, CommsProvider: Operator);

                    if (Operator == "CM")
                    {
                        CLH.DownloadAndCreateCMSMSLog(MessageSid, (DateTimeOffset)MsgRslt.CreatedOn);
                    }
                    else if (Operator == "TWILIO")
                    {
                        CLH.DownloadAndCreateTwilioLog(MessageSid, "TEXT");
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

        public async Task<string> HandelTwoFactor(string MessageSid, string SmsStatus)
        {

            try
            {
                string Message = "";
                var tfrec =  _context.Set<TwoFactorAuthLog>()
                             .Where(TF=> TF.CloudMessageId == MessageSid).FirstOrDefault();

                if (tfrec != null)
                {
                    tfrec.Status = SmsStatus;
                    _context.SaveChanges();

                    
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
        public async Task<string > TwilioCallAck()
        {
            try
            {
                string emailmessage = "There was an error while processing the delivery confirmation";
                emailmessage = "application/xml";
                string CloudMessageId =_httpContextAccessor.HttpContext.Request["CallSid"] == null ? "" : context.Request["CallSid"].Trim();
                string Digits = context.Request["Digits"] == null ? "" : context.Request["Digits"].Trim();
                var MsgRslt = (from MD in _context.Set<MessageDevice>()
                               join ML in _context.Set<MessageList>() on MD.MessageListId equals ML.MessageListId
                               join U in _context.Set<User>() on ML.RecepientUserId equals U.UserId
                               where MD.CloudMessageId == CloudMessageId
                               select new { MD, ML, U }).FirstOrDefault();
                if (MsgRslt != null)
                {

                    string TimeZoneId = DBC.GetTimeZoneVal(MsgRslt.U.UserId);

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

                    //var txtlink = (from TL in db.TextMessageLink where TL.MessageListId == MsgRslt.ML.MessageListId select TL).FirstOrDefault();
                    //if(txtlink != null) {
                    //    txtlink.Acknowledged = true;
                    //}

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
    }
}
