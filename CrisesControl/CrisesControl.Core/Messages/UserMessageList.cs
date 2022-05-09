using CrisesControl.Core.Incidents;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages {
    public class UserMessageList {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public UserFullName SentBy {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        }
        public bool MultiResponse { get; set; }
        public int Priority { get; set; }
        public int TotalAck { get; set; }
        public int TotalNotAck { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int AttachmentCount { get; set; }
        public int HasReply { get; set; }
    }
}
