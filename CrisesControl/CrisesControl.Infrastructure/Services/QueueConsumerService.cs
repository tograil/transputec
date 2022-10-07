
using CrisesControl.Core.Common;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Context.Misc;
using GrpcAuditLogClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public  class QueueConsumerService: IQueueConsumerService
    {

        public List<string> RabbitHost = new List<string>();
        public int MaxQueueConsumer = 3;
        public int MaxConsumerItem = 1;
        public string RabbitQueueExchange = "cc_processing_exchange";
        public string RabbitMQUser = "guest";
        public string RabbitMQPassword = "guest";
        public string RabbitVirtualHost = "/";

        public bool isFundAvailable = true;
        public ushort RabbitMQHeartBeat = 60;
        private  readonly IHttpContextAccessor _httpContextAccessor;    
        private  readonly CrisesControlContext _db;
        private  readonly IDBCommonRepository _DBC;
        private  readonly IMessageService _MSG;
        private readonly IQueueMessageService _queueHelper;

        public QueueConsumerService(CrisesControlContext db, IHttpContextAccessor httpContextAccessor, IDBCommonRepository DBC, IMessageService MSG, IQueueMessageService queue)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _DBC = DBC;
            _MSG = MSG;
            _queueHelper = queue;
        }
        public bool IsFundAvailable { 
            get=> isFundAvailable; 
            set=> isFundAvailable=value; }
        public  async Task<int> CreateMessageList(int messageID, string replyTo = "")
        {

         
            string MessageType = string.Empty;
            int Priority = 999;
            int QueueSize = 0;
            int CascadePlanID = 0;
            try
            {
               

                    //_db.Database.ti = 300;

                    var msg = _db.Set<Message>().Where(M=> M.MessageId == messageID).FirstOrDefault();
                    if (msg != null)
                    {

                        CascadePlanID = msg.CascadePlanId;

                        string TimeZoneId = await _DBC.GetTimeZoneVal(msg.CreatedBy);

                        bool notifySender = false;
                        bool.TryParse(await _DBC.GetCompanyParameter("INC_SENDER_INCIDENT_UPDATE", msg.CompanyId), out notifySender);

                        bool NotifyKeyholders = false;
                        bool.TryParse(await _DBC.GetCompanyParameter("NOTIFY_KEYHOLDER_BY_PING", msg.CompanyId), out NotifyKeyholders);

                        MessageType = msg.MessageType.ToUpper();
                        Priority = msg.Priority;

                        string rpl_sp_name = "Pro_Create_Reply_Message_List ";

                        if (CascadePlanID > 0)
                            rpl_sp_name = "Pro_Create_Message_Queue_Cascading ";


                        if (msg.MessageType.ToUpper() == "PING")
                        {
                            //Check for incident closure ping.
                            if (NotifyKeyholders && msg.IncidentActivationId > 0 && msg.Source != 7)
                            {
                                try
                                {
                                    var pMessageId = new SqlParameter("@MessageID", msg.MessageId);
                                    var pCustomerTime = new SqlParameter("@CustomerTime", _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                                    _db.Database.ExecuteSqlRaw("Pro_Create_Keyholder_Message_List @MessageID, @CustomerTime", pMessageId, pCustomerTime);
                                }
                                catch (Exception ex)
                                {
                                   
                                    throw ex;
                                }
                            }
                            else
                            { //Normal ping message
                                var pMessageId = new SqlParameter("@MessageID", msg.MessageId);
                                var pNotifySender = new SqlParameter("@NotifySender", notifySender);
                                var pCustomerTime = new SqlParameter("@CustomerTime", _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                                if (msg.ParentId == 0 && replyTo.ToUpper() != "RENOTIFY")
                                {
                                    try
                                    {
                                        _db.Database.ExecuteSqlRaw("Pro_Create_Message_List @MessageID, @NotifySender,@CustomerTime", pMessageId, pNotifySender, pCustomerTime);
                                    }
                                    catch (Exception ex)
                                    {
                                     
                                        throw ex;
                                    }

                                }
                                else
                                {
                                    var pReplyTo = new SqlParameter("@ReplyTo", replyTo);
                                    try
                                    {
                                        var result = _db.Set<Result>().FromSqlRaw(rpl_sp_name + " @MessageID, @ReplyTo, @CustomerTime", pMessageId, pReplyTo, pCustomerTime).FirstOrDefault();
                                        if (result != null)
                                        {
                                            QueueSize = result.ResultID;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                       
                                        throw ex;
                                    }
                                }
                            }
                        }
                        else if (msg.MessageType.ToUpper() == "INCIDENT")
                        {

                            var pIncidentActivationId = new SqlParameter("@IncidentActivationID", msg.IncidentActivationId);
                            var pMessageId = new SqlParameter("@MessageID", msg.MessageId);
                            var pNotifySender = new SqlParameter("@NotifySender", notifySender);
                            var pCustomerTime = new SqlParameter("@CustomerTime", _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                            if (msg.ParentId == 0 && replyTo.ToUpper() != "RENOTIFY")
                            {
                                try
                                {
                                    _db.Database.ExecuteSqlRaw("Pro_Create_Incident_Message_List @IncidentActivationID, @MessageID, @NotifySender,@CustomerTime",
                                            pIncidentActivationId, pMessageId, pNotifySender, pCustomerTime);
                                }
                                catch (Exception ex)
                                {
                                    
                                    throw ex;
                                }

                            }
                            else
                            {
                                try
                                {
                                    var pReplyTo = new SqlParameter("@ReplyTo", replyTo);
                                    var result = _db.Set<Result>().FromSqlRaw(rpl_sp_name + " @MessageID, @ReplyTo, @CustomerTime", pMessageId, pReplyTo, pCustomerTime).FirstOrDefault();
                                    if (result != null)
                                    {
                                        QueueSize = result.ResultID;
                                    }
                                }
                                catch (Exception ex)
                                {
                                  
                                    throw ex;
                                }
                            }
                        }

                       await  CreateCascadingJobs(CascadePlanID, messageID, msg.IncidentActivationId, msg.CompanyId, TimeZoneId);

                       await  _DBC.MessageProcessLog(msg.MessageId, "MESSAGE_LIST_CREATED", "", "", "Total count: " + QueueSize);

                        IsFundAvailable =await _MSG.CalculateMessageCost(msg.CompanyId, messageID, msg.MessageText);
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (replyTo.ToUpper() != "RENOTIFY")
                {
                   await Task.Factory.StartNew(() => _queueHelper.MessageDeviceQueue(messageID, MessageType, 1, CascadePlanID));
                }
                else
                {
                  await   Task.Factory.StartNew(() => _queueHelper.MessageDevicePublish(messageID, 1, ""));
                }
                //CreateMessageDevice(MessageID);
            }
            return QueueSize;
        }
        public  async Task CreateCascadingJobs(int planID, int messageID, int activeIncidentID, int companyID, string timeZoneId)
        {
           
            try
            {
                if (planID > 0)
                {

                   
                        var steps = (from PI in _db.Set<PriorityInterval>()
                                     where PI.CascadingPlanId == planID && PI.CompanyId == companyID && PI.Interval > 0 && PI.Priority > 1
                                     select PI).ToList();

                        ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                        IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                        DateTimeOffset StartMessageTime =await  _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                        int LastPriority = 1;

                        foreach (var step in steps)
                        {
                            LastPriority = step.Priority;
                            StartMessageTime = StartMessageTime.AddMinutes(step.Interval);

                            string jobName = "MESSAGE_ATTEMPT_" + messageID + "_" + step.Priority;
                            var jobKey = new JobKey(jobName, "MESSAGE_CASCADE_" + activeIncidentID);
                            bool jobExists = false;
                            int tried = 0;
                            while (jobExists == false && tried < 5)
                            {
                                if (!_scheduler.CheckExists(jobKey).Result)
                                {
                                    CreateCascadeJobStep(messageID, planID, activeIncidentID, step.MessageType, step.Priority, step.Interval, ref StartMessageTime, timeZoneId);
                                }
                                else
                                {
                                    jobExists = true;
                                }
                                tried++;
                            }
                        }

                        //Schedule the SOS Launch of users.
                        var sos = await _db.Set<CascadingPlan>().Where(w => w.PlanId == planID).FirstOrDefaultAsync();
                        if (sos != null)
                        {
                            if (sos.LaunchSos)
                            {
                                StartMessageTime = StartMessageTime.AddMinutes(sos.LaunchSosinterval);
                                int sospriority = LastPriority + 1;
                               await  SOSSchedule(messageID, companyID, sospriority, StartMessageTime, timeZoneId, activeIncidentID);
                            }
                        }
                    }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void CreateCascadeJobStep(int messageID, int planID, int activeIncidentID, string messageType, int priority, int interval, ref DateTimeOffset startMessageTime, string timeZoneId)
        {
           
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "MESSAGE_ATTEMPT_" + messageID + "_" + priority;
                string taskTrigger = "MESSAGE_ATTEMPT_" + messageID + "_" + priority;

                IJobDetail jobDetail = JobBuilder.Create<CascadeMessageJob>().WithIdentity(jobName, "MESSAGE_CASCADE_" + activeIncidentID).Build();

                jobDetail.JobDataMap["PlanId"] = planID;
                jobDetail.JobDataMap["MessageId"] = messageID;
                jobDetail.JobDataMap["Priority"] = priority;
                jobDetail.JobDataMap["MessageType"] = messageType;
                string dayLight = _DBC.IsDayLightOn(startMessageTime.DateTime).ToString();
                bool isDayLightOn = Convert.ToBoolean(dayLight);
                if (!isDayLightOn)
                {
                    string date = _DBC.ConvertToLocalTime("GMT Standard Time", startMessageTime.ToUniversalTime().DateTime).ToString();
                    startMessageTime =DateTimeOffset.Parse(date);
                }

                string CronExpressionStr = startMessageTime.Second + " " + startMessageTime.Minute + " " + startMessageTime.Hour + " " + startMessageTime.Day + " " + startMessageTime.Month + " ? " + startMessageTime.Year;

                var trigger = TriggerBuilder.Create()
                                             .WithIdentity(taskTrigger, "MESSAGE_CASCADE_" + activeIncidentID)
                                             .WithSimpleSchedule(
                                                x => x.WithMisfireHandlingInstructionFireNow())
                                                .ForJob(jobDetail)
                                                .StartAt(startMessageTime)
                                                .Build();

                var triggered = _scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async  Task SOSSchedule(int messageID, int companyID, int priority, DateTimeOffset startTime, string timeZoneId, int activeIncidentID)
        {
            
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

               

                    string jobName = "CASCADE_SOS_" + messageID + "_" + priority;
                    string taskTrigger = "CASCADE_SOS_" + messageID + "_" + priority;

                    IJobDetail jobDetail = JobBuilder.Create<SOSCascadeMessageJob>().WithIdentity(jobName, "MESSAGE_CASCADE_" + activeIncidentID).Build();

                    jobDetail.JobDataMap["MessageID"] = messageID;
                    jobDetail.JobDataMap["CompanyID"] = companyID;

                
                bool isDayLightOn =await _DBC.IsDayLightOn(startTime.DateTime);
                if (!isDayLightOn)
                {
                    
                    startTime =await _DBC.ConvertToLocalTime("GMT Standard Time", startTime.ToUniversalTime().DateTime);
                }

                string CronExpressionStr = startTime.Second + " " + startTime.Minute + " " + startTime.Hour + " " + startTime.Day + " " + startTime.Month + " ? " + startTime.Year;

                    var trigger = TriggerBuilder.Create()
                                                 .WithIdentity(taskTrigger, "MESSAGE_CASCADE_" + activeIncidentID)
                                                 .WithCronSchedule(CronExpressionStr, x => x
                                                     .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
                                                     .WithMisfireHandlingInstructionDoNothing())
                                                 .ForJob(jobDetail)
                                                 .Build();

                   await _scheduler.ScheduleJob(jobDetail, trigger);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
