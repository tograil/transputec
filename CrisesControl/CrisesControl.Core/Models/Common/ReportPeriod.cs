namespace CrisesControl.Core.Models.Common
{
    public class ReportPeriod: CCBase
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public int SelectedUserID { get; set; }

        public int FilterRelation { get; set; }
        public int ObjectMapId { get; set; }
        public int MessageId { get; set; }
        public string MessageType { get; set; }
    }
}
