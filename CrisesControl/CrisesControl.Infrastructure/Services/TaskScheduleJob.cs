using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Messages;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class TaskScheduleJob : IJob
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CrisesControlContext _context;
        private readonly IActiveIncidentRepository _activeIncidentRepository;
        public TaskScheduleJob(CrisesControlContext controlContext, IHttpContextAccessor httpContextAccessor, IActiveIncidentRepository activeIncidentRepository)
        {
            this._context = controlContext;
            this._httpContextAccessor = httpContextAccessor;
            this._activeIncidentRepository = activeIncidentRepository;
        }
        //ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
        //private static IScheduler _scheduler;
        public async Task Execute(IJobExecutionContext context)
        {

            int ActiveIncidentTaskID = context.JobDetail.JobDataMap.GetInt("ActiveIncidentTaskID");
            string Action = context.JobDetail.JobDataMap.GetString("ACTION");
            string escalation_type = string.Empty;
            DBCommon DBC = new DBCommon(_context,_httpContextAccessor);
            try
            {
                var task = await  _context.Set<TaskActiveIncident>().Include(AI=>AI.IncidentActivation)
                            .Where(TS=> TS.ActiveIncidentTaskId == ActiveIncidentTaskID
                            ).FirstOrDefaultAsync();
                if (task != null)
                {
                    if (task.TaskStatus != 7)
                    {
                        string TimeZoneId = DBC.GetTimeZoneByCompany(task.CompanyId);
                        DateTimeOffset checkTime = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        if (task.TaskStatus == 1 && task.TaskEscalatedDate.Year < 2000 && task.TaskStatus != 6)
                        {
                            checkTime = task.TaskActivationDate.AddMinutes(task.EscalationDuration);
                        }
                        else if (new List<int> { 2, 3, 4, 5 }.Contains(task.TaskStatus) && task.TaskEscalatedDate.Year < 2000)
                        {
                            checkTime = task.TaskAcceptedDate.AddMinutes(task.ExpectedCompletionTime);
                        }

                        //DateTimeOffset GMTTime = DBC.GetServerTime(TimeZoneId, checkTime);

                        //if(GMTTime <= DateTime.Now) {
                        if (DateTimeOffset.Compare(checkTime, DateTime.Now.GetDateTimeOffset()) <= 0)
                        {

                            task.TaskEscalatedDate = DateTime.Now.GetDateTimeOffset( TimeZoneId);

                            if (Action == "ESCALATION")
                            {
                                task.DelayedAccept = task.TaskEscalatedDate;
                                escalation_type = "not accepted on time";
                                task.TaskStatus = 6;
                            }

                            if (Action == "COMPLETION")
                            {
                                task.DelayedComplete = task.TaskEscalatedDate;
                                escalation_type = "not completed on time";
                            }
                            _context.Update(task);
                            await _context.SaveChangesAsync();
                           
                            List<string> groups = new List<string>();
                            groups.Add("ESCALATION");

                            //Add task action history
                            string task_action = "Task has been escalated because it was " + escalation_type;
                           await  _activeIncidentRepository.AddTaskAction(ActiveIncidentTaskID, task_action, task.UpdatedBy, 6, TimeZoneId);

                            string comms_method = DBC.GetCompanyParameter("TASK_SYSTEM_COMMS_METHOD", task.CompanyId);
                            List<int> commslist = comms_method.Split(',').Select(int.Parse).ToList();

                            int[] CommsMethod = null;
                            var commsmethod = await _context.Set<MessageMethod>()
                                               .Where(MM=> MM.ActiveIncidentId == task.ActiveIncidentId && commslist.Contains(MM.MethodId) ||
                                               commslist.Contains(MM.MethodId))
                                               .Select(MM=> MM.MethodId).Distinct().ToArrayAsync();
                            if (commsmethod != null)
                            {
                                CommsMethod = commsmethod;
                            }

                            string action_update = "Task " + task.TaskSequence + ": \"" + CrisesControl.SharedKernel.Utils.StringExtensions.Truncate(task.TaskTitle, 70) + "\" " + escalation_type;
                           _activeIncidentRepository.send_notifiation_to_groups(groups, task.ActiveIncidentId, task.ActiveIncidentTaskId, action_update, task.UpdatedBy, task.CompanyId, TimeZoneId, false, 4, CommsMethod, sourceAction: SourceAction.TaskEscalation);
                        }
                    }
                    else
                    {
                        //Delete Scheduled jobs for the incident
                        DBC.DeleteScheduledJob("START_ACPT_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");
                        DBC.DeleteScheduledJob("START_ESCL_TASK_" + task.ActiveIncidentTaskId, "TASK_SCHEDULE");
                    }
                }
                 await Task.WhenAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
