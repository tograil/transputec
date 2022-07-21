using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class ImportDumpResult
    {
        public int ImportDumpId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string SessionId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string ISD { get; set; }
        public string Phone { get; set; }
        public string LLISD { get; set; }
        public string Landline { get; set; }
        public string UserRole { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string Group { get; set; }
        public string GroupStatus { get; set; }
        public string GroupAction { get; set; }
        public string Department { get; set; }
        public string DepartmentStatus { get; set; }
        public string DepartmentAction { get; set; }
        public string Location { get; set; }
        public string LocationAddress { get; set; }
        public string LocationStatus { get; set; }
        public string LocationAction { get; set; }
        public int SecurityGroupId { get; set; }
        public string Security { get; set; }
        public string PingMethods { get; set; }
        public string IncidentMethods { get; set; }
        public string EmailCheck { get; set; }
        public string LocationCheck { get; set; }
        public string GroupCheck { get; set; }
        public string DepartmentCheck { get; set; }
        public string SecurityCheck { get; set; }
        public string ImportAction { get; set; }
        public string ActionType { get; set; }
        public string ActionCheck { get; set; }
        public string ValidationMessage { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int LocationId { get; set; }
        public int GroupId { get; set; }
        public string LocLat { get; set; }
        public string LocLng { get; set; }
        public string ImportStatus { get; set; }
    }

    public class ImportSummary
    {
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalImport { get; set; }
        public int TotalUpdate { get; set; }
        public int TotalSkip { get; set; }
        public int TotalDelete { get; set; }
        public string ResultFile { get; set; }
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        }
    }

    public class EmailNotificationToUser
    {
        public string ToEMail { get; set; }
        public string MessageBody { get; set; }
        public string FromEMail { get; set; }
        public string HostName { get; set; }
        public string Subject { get; set; }
    }
}
