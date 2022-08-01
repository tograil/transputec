using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class IncidentUserMessageResponse
    {
        public DateTimeOffset MessageSent { get; set; }
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
        [NotMapped]
        public UserFullName SentBy { get; set; }
        public DateTimeOffset MessageAcknowledge { get; set; }
        public int IsAck { get; set; }
        public string MessageText { get; set; }
        public string AckMethod { get; set; }
        public bool IsTaskRecipient { get; set; }
        public int Source { get; set; }
        public string MessageLat { get; set; }
        public string MessageLng { get; set; }
    }
}
