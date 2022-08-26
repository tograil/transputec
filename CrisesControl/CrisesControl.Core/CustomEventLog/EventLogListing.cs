using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CustomEventLog
{
    public class EventLogListing
    {
        public long EventLogID { get; set; }
        public int EventLogHeaderID { get; set; }
        public int SrNo { get; set; }
        public DateTimeOffset LogEntryDateTime { get; set; }
        public string IncidentDetails { get; set; }
        public string SourceOfInformation { get; set; }
        public Nullable<int> IsConfirmed { get; set; }
        public string CMTAction { get; set; }
        public Nullable<int> ActionPriority { get; set; }
        public Nullable<int> ActionUser { get; set; }
        public Nullable<int> ActionGroup { get; set; }
        public Nullable<DateTimeOffset> ActionDueBy { get; set; }
        public Nullable<int> StatusOfAction { get; set; }
        public string ActionDetail { get; set; }
        public string Comments { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string UpdatedFirstName { get; set; }
        public string UpdatedLastName { get; set; }
        public UserFullName LastUpdateBy
        {
            get { return new UserFullName { Firstname = UpdatedFirstName, Lastname = UpdatedLastName }; }
            set { new UserFullName { Firstname = UpdatedFirstName, Lastname = UpdatedLastName }; }
        }
        public string ActionUserFirstName { get; set; }
        public string ActionUserLastName { get; set; }
        public UserFullName ActionByUser
        {
            get { return new UserFullName { Firstname = ActionUserFirstName, Lastname = ActionUserLastName }; }
            set { new UserFullName { Firstname = ActionUserFirstName, Lastname = ActionUserLastName }; }
        }
        public Nullable<DateTimeOffset> ActionedDate { get; set; }
        public string AccessLevel { get; set; }
    }
}
