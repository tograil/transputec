using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.System
{
    public class AuditList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RecordFirstName { get; set; }
        public string RecordLastName { get; set; }
        [NotMapped]
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { }
        }
        [NotMapped]
        public UserFullName RecordUserName
        {
            get { return new UserFullName { Firstname = RecordFirstName, Lastname = RecordLastName }; }
            set { }
        }

        public string NewValue { get; set; }
        public string EventType { get; set; }
        public string OriginalValue { get; set; }
        public string ColumnName { get; set; }
        public DateTimeOffset EventDateUTC { get; set; }
        public int RecordId { get; set; }
        public string EventString { get; set; }
    }
}
