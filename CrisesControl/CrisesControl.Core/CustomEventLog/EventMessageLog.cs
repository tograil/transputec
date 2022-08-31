using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CustomEventLog
{
    public class EventMessageLog
    {
        public string MessageText { get; set; }
        public string SentByFirstName { get; set; }
        public string SentByLastName { get; set; }
        public UserFullName SentByName
        {
            get { return new UserFullName { Firstname = SentByFirstName, Lastname = SentByLastName }; }
            set { new UserFullName { Firstname = SentByLastName, Lastname = SentByLastName }; }
        }
        public string RcpntByFirstName { get; set; }
        public string RcpntByLastName { get; set; }
        public UserFullName RcpntName
        {
            get { return new UserFullName { Firstname = RcpntByFirstName, Lastname = RcpntByLastName }; }
            set { new UserFullName { Firstname = RcpntByLastName, Lastname = RcpntByLastName }; }
        }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
