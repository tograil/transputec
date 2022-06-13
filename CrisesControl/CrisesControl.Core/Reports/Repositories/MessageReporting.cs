using CrisesControl.Core.Compatibility;

namespace CrisesControl.Core.Reports.Repositories
{
    public class MessageReporting: ReportPeriod
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
        public string MessageType { get; set; }
        public int DrillOpt { get; set; }
        public int GroupId { get; set; }
        public int ObjectMappingId { get; set; }
        public int FilterUser { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public bool ShowDeletedGroups { get; set; }
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public int SelectedUserID { get; set; }
        public int FilterRelation { get; set; }
        public int ObjectMapId { get; set; }
        public int CurrentUserId { get; set; }
        public int CompanyId { get; set; }
    }
   
   

 
}
