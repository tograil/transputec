﻿using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Common;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
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
    public class QueueConsumer
    {

        public List<string> RabbitHost;
        public int MaxQueueConsumer = 3;
        public int MaxConsumerItem = 1;
        public string RabbitQueueExchange = "cc_processing_exchange";
        public string RabbitMQUser = "guest";
        public string RabbitMQPassword = "guest";
        public string RabbitVirtualHost = "/";

        public static bool IsFundAvailable = true;
        public ushort RabbitMQHeartBeat = 60;
        private static readonly CrisesControlContext _db;
        private static readonly IHttpContextAccessor _httpContextAccessor;
       


        
        public static async Task<int> CreateMessageList(int MessageID, string ReplyTo = "")
        {

            DBCommon DBC = new DBCommon(_db, _httpContextAccessor);
            string MessageType = string.Empty;
            int Priority = 999;
            int QueueSize = 0;
            int CascadePlanID = 0;
            try
            {
               

                    //_db.Database.ti = 300;

                    var msg = _db.Set<Message>().Where(M=> M.MessageId == MessageID).FirstOrDefault();
                    if (msg != null)
                    {

                        CascadePlanID = msg.CascadePlanId;

                        string TimeZoneId = DBC.GetTimeZoneVal(msg.CreatedBy);

                        bool notifySender = false;
                        bool.TryParse(DBC.GetCompanyParameter("INC_SENDER_INCIDENT_UPDATE", msg.CompanyId), out notifySender);

                        bool NotifyKeyholders = false;
                        bool.TryParse(DBC.GetCompanyParameter("NOTIFY_KEYHOLDER_BY_PING", msg.CompanyId), out NotifyKeyholders);

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
                                    var pCustomerTime = new SqlParameter("@CustomerTime", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

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
                                var pCustomerTime = new SqlParameter("@CustomerTime", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                                if (msg.ParentId == 0 && ReplyTo.ToUpper() != "RENOTIFY")
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
                                    var pReplyTo = new SqlParameter("@ReplyTo", ReplyTo);
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
                            var pCustomerTime = new SqlParameter("@CustomerTime", DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId));

                            if (msg.ParentId == 0 && ReplyTo.ToUpper() != "RENOTIFY")
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
                                    var pReplyTo = new SqlParameter("@ReplyTo", ReplyTo);
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

                        CreateCascadingJobs(CascadePlanID, MessageID, msg.IncidentActivationId, msg.CompanyId, TimeZoneId);

                        DBC.MessageProcessLog(msg.MessageId, "MESSAGE_LIST_CREATED", "", "", "Total count: " + QueueSize);

                        Messaging MSG = new Messaging(_db, _httpContextAccessor);
                        IsFundAvailable =await MSG.CalculateMessageCost(msg.CompanyId, MessageID, msg.MessageText);
                    }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ReplyTo.ToUpper() != "RENOTIFY")
                {
                    Task.Factory.StartNew(() => QueueHelper.MessageDeviceQueue(MessageID, MessageType, 1, CascadePlanID));
                }
                else
                {
                    Task.Factory.StartNew(() => QueueHelper.MessageDevicePublish(MessageID, 1, ""));
                }
                //CreateMessageDevice(MessageID);
            }
            return QueueSize;
        }
        public static async void CreateCascadingJobs(int PlanID, int MessageID, int ActiveIncidentID, int CompanyID, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(_db, _httpContextAccessor);
            try
            {
                if (PlanID > 0)
                {

                   
                        var steps = (from PI in _db.Set<PriorityInterval>()
                                     where PI.CascadingPlanId == PlanID && PI.CompanyId == CompanyID && PI.Interval > 0 && PI.Priority > 1
                                     select PI).ToList();

                        ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                        IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                        DateTimeOffset StartMessageTime = DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);
                        int LastPriority = 1;

                        foreach (var step in steps)
                        {
                            LastPriority = step.Priority;
                            StartMessageTime = StartMessageTime.AddMinutes(step.Interval);

                            string jobName = "MESSAGE_ATTEMPT_" + MessageID + "_" + step.Priority;
                            var jobKey = new JobKey(jobName, "MESSAGE_CASCADE_" + ActiveIncidentID);
                            bool jobExists = false;
                            int tried = 0;
                            while (jobExists == false && tried < 5)
                            {
                                if (!_scheduler.CheckExists(jobKey).Result)
                                {
                                    CreateCascadeJobStep(MessageID, PlanID, ActiveIncidentID, step.MessageType, step.Priority, step.Interval, ref StartMessageTime, TimeZoneId);
                                }
                                else
                                {
                                    jobExists = true;
                                }
                                tried++;
                            }
                        }

                        //Schedule the SOS Launch of users.
                        var sos = await _db.Set<CascadingPlan>().Where(w => w.PlanId == PlanID).FirstOrDefaultAsync();
                        if (sos != null)
                        {
                            if (sos.LaunchSos)
                            {
                                StartMessageTime = StartMessageTime.AddMinutes(sos.LaunchSosinterval);
                                int sospriority = LastPriority + 1;
                                SOSSchedule(MessageID, CompanyID, sospriority, StartMessageTime, TimeZoneId, ActiveIncidentID);
                            }
                        }
                    }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateCascadeJobStep(int MessageID, int PlanID, int ActiveIncidentID, string MessageType, int Priority, int Interval, ref DateTimeOffset StartMessageTime, string TimeZoneId)
        {
            DBCommon DBC = new DBCommon(_db,_httpContextAccessor);
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

                string jobName = "MESSAGE_ATTEMPT_" + MessageID + "_" + Priority;
                string taskTrigger = "MESSAGE_ATTEMPT_" + MessageID + "_" + Priority;

                IJobDetail jobDetail = JobBuilder.Create<CascadeMessageJob>().WithIdentity(jobName, "MESSAGE_CASCADE_" + ActiveIncidentID).Build();

                jobDetail.JobDataMap["PlanId"] = PlanID;
                jobDetail.JobDataMap["MessageId"] = MessageID;
                jobDetail.JobDataMap["Priority"] = Priority;
                jobDetail.JobDataMap["MessageType"] = MessageType;

                if (!DBC.IsDayLightOn(StartMessageTime.DateTime))
                {
                    StartMessageTime = DBC.ConvertToLocalTime("GMT Standard Time", StartMessageTime.ToUniversalTime().DateTime);
                }

                string CronExpressionStr = StartMessageTime.Second + " " + StartMessageTime.Minute + " " + StartMessageTime.Hour + " " + StartMessageTime.Day + " " + StartMessageTime.Month + " ? " + StartMessageTime.Year;

                var trigger = TriggerBuilder.Create()
                                             .WithIdentity(taskTrigger, "MESSAGE_CASCADE_" + ActiveIncidentID)
                                             .WithSimpleSchedule(
                                                x => x.WithMisfireHandlingInstructionFireNow())
                                                .ForJob(jobDetail)
                                                .StartAt(StartMessageTime)
                                                .Build();

                var triggered = _scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SOSSchedule(int MessageID, int CompanyID, int Priority, DateTimeOffset StartTime, string TimeZoneId, int ActiveIncidentID)
        {
            DBCommon DBC = new DBCommon(_db, _httpContextAccessor);
            try
            {
                ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
                IScheduler _scheduler = schedulerFactory.GetScheduler().Result;

               

                    string jobName = "CASCADE_SOS_" + MessageID + "_" + Priority;
                    string taskTrigger = "CASCADE_SOS_" + MessageID + "_" + Priority;

                    IJobDetail jobDetail = JobBuilder.Create<SOSCascadeMessageJob>().WithIdentity(jobName, "MESSAGE_CASCADE_" + ActiveIncidentID).Build();

                    jobDetail.JobDataMap["MessageID"] = MessageID;
                    jobDetail.JobDataMap["CompanyID"] = CompanyID;

                    if (!DBC.IsDayLightOn(StartTime.DateTime))
                    {
                        StartTime = DBC.ConvertToLocalTime("GMT Standard Time", StartTime.ToUniversalTime().DateTime);
                    }

                    string CronExpressionStr = StartTime.Second + " " + StartTime.Minute + " " + StartTime.Hour + " " + StartTime.Day + " " + StartTime.Month + " ? " + StartTime.Year;

                    var trigger = TriggerBuilder.Create()
                                                 .WithIdentity(taskTrigger, "MESSAGE_CASCADE_" + ActiveIncidentID)
                                                 .WithCronSchedule(CronExpressionStr, x => x
                                                     .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId))
                                                     .WithMisfireHandlingInstructionDoNothing())
                                                 .ForJob(jobDetail)
                                                 .Build();

                    _scheduler.ScheduleJob(jobDetail, trigger);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}