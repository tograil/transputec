using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Incidents
{
    public class SourceAction
    {
        public const string IncidentLaunch = "INCIDENT_LAUNCH";
        public const string SosLaunch = "SOS_LAUNCH";
        public const string IncidentTest = "INCIDENT_TEST";
        public const string IncidentUpdate = "INCIDENT_UPDATE";
        public const string SosUpdate = "SOS_UPDATE";
        public const string IncidentClosure = "INCIDENT_CLOSE";
        public const string SosClosure = "SOS_CLOSE";
        public const string NewAdHocTask = "NEW_TASK";
        public const string TaskAccepted = "TASK_ACCEPTED";
        public const string TaskCompleted = "TASK_COMPLETED";
        public const string TaskAvailable = "TASK_AVAILABLE";
        public const string TaskUpdate = "TASK_UPDATE";
        public const string TaskEscalation = "TASK_ESCALATION";
        public const string TaskTakeOver = "TASK_TAKEOVER";
        public const string TaskDeclined = "TASK_DECLINED";
        public const string TaskReallocated = "TASK_REALLOCATED";
        public const string TaskDelegated = "TASK_DELEGATED";
        public const string TaskReassigned = "TASK_REASSIGNED";
        public const string Ping = "PING";
        public const string PingReply = "PING_REPLY";
        public const string PublicAlert = "PUBLIC_ALERT";
        public const string EventLogNotify = "EVENT_LOG_NOTIFY";
    }
}
