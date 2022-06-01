using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class IncidentMessageAuditResponse
    {
        public int MessageId { get; set; }
        public int Source { get; set; }
        public string MessageText { get; set; }
        public int AttachmentCount { get; set; }
        public string SentByFirstName
        {
            get
            {
                return SentBy?.Firstname ?? null;
            }
            set
            {
                if (SentBy == null)
                {
                    SentBy = new UserFullName();
                }
                if (SentBy.Firstname != value)
                {
                    SentBy.Firstname = value;
                }
            }
        }
        public string SentByLastName
        {
            get
            {
                return SentBy?.Lastname ?? null;
            }
            set
            {
                if (SentBy == null)
                {
                    SentBy = new UserFullName();
                }
                if (SentBy.Lastname != value)
                {
                    SentBy.Lastname = value;
                }
            }
        }
        public UserFullName SentBy { get; set; }
        public int TotalAcknowledged { get; set; }
        public int TotalNotAcknowledged { get; set; }
        public DateTimeOffset MessageCreatedOn { get; set; }
    }
}
