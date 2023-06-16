using CrisesControl.Core.Users;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog
{
    public class GetEventLogResponse
    {
        public long EventLogHeaderID { get; set; }
        public int ActiveIncidentID { get; set; }
        public string IncidentName { get; set; }
        public DateTimeOffset LaunchedOn { get; set; }
        public DateTimeOffset ClosedOn { get; set; }
        public string LaunchedFirstName { get; set; }
        public string LaunchedLastName { get; set; }
        public UserFullName LaunchedByName
        {
            get { return new UserFullName { Firstname = LaunchedFirstName, Lastname = LaunchedLastName }; }
            set { new UserFullName { Firstname = LaunchedFirstName, Lastname = LaunchedLastName }; }
        }
        public string DepartmentName { get; set; }
        public string LogCreatedFirstName { get; set; }
        public string LogCreatedLastName { get; set; }
        public UserFullName LogCreatedBy
        {
            get { return new UserFullName { Firstname = LogCreatedFirstName, Lastname = LogCreatedLastName }; }
            set { new UserFullName { Firstname = LogCreatedFirstName, Lastname = LogCreatedLastName }; }
        }
        public string IncidentIcon { get; set; }
        public Nullable<int> PermittedDepartment { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string AccessLevel { get; set; }
    }
}
