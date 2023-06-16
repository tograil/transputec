using System;
using System.Collections.Generic;

namespace CrisesControl.Core.Import
{
    public class ImportDumpModel
    {
        public string SessionId { get; set; }
        public string JobType { get; set; }
        public List<ImportDumpData> ImportDumpData { get; set; }
    }

    public class ImportDumpData
    {
        public ImportDumpData()
        {
            Action = "ADD";
            Group = "";
            GroupStatus = "ACTIVE";
            Department = "";
            DepartmentStatus = "ACTIVE";
            Location = "";
            LocationAddress = "";
            LocationStatus = "ACTIVE";
            MenuAccess = "";
            PingMethods = "";
            IncidentMethods = "";
            SecurityDescription = "";
            LocationAction = "ADD";
            GroupAction = "ADD";
            DepartmentAction = "ADD";
            UserRole = "User";
            Status = "ACTIVE";
            ActionType = "";
        }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string SessionId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string EncryptedEmail { get; set; }
        public string MobileISD { get; set; }
        public string Mobile { get; set; }
        public string ActionType { get; set; }
        public string ISDLandline { get; set; }
        public string Landline { get; set; }
        public string UserRole { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string Group { get; set; }
        public string GroupStatus { get; set; }
        public string Department { get; set; }
        public string DepartmentStatus { get; set; }
        public string Location { get; set; }
        public string LocationAddress { get; set; }
        public string LocationStatus { get; set; }
        public string MenuAccess { get; set; }
        public string PingMethods { get; set; }
        public string IncidentMethods { get; set; }
        public string SecurityDescription { get; set; }
        public string LocationAction { get; set; }
        public string GroupAction { get; set; }
        public string DepartmentAction { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }

}