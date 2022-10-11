using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Queues.Services
{
    public interface IQueueConsumerService
    {
        public bool IsFundAvailable { get; set; }
        Task<int> CreateMessageList(int MessageID, string ReplyTo = "", bool SendToAllRecipient = false);
        Task CreateCascadingJobs(int planID, int messageID, int activeIncidentID, int companyID, string timeZoneId);
        void CreateCascadeJobStep(int messageID, int planID, int activeIncidentID, string messageType, int priority, int interval, ref DateTimeOffset startMessageTime, string timeZoneId);
        Task SOSSchedule(int messageID, int companyID, int priority, DateTimeOffset startTime, string timeZoneId, int activeIncidentID);
    }
}
