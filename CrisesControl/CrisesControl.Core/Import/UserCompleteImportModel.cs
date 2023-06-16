using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class UserCompleteImportModel
    {
        public string SessionId { get; set; }
        public string JobType { get; set; }

        public List<UserCompData> UserCompData { get; set; }
    }

    public class UserCompData
    {
        public UserCompData()
        {
            Action = "ADD";
            Department = "";
            DepartmentStatus = 1;
            Location = "";
            LocationAddress = "";
            LocationStatus = 1;
            Security = "";
            PingMethods = "";
            IncidentMethods = "";
            SecurityDescription = "";
            LocationAction = "ADD";
            DepartmentAction = "ADD";
            UserRole = "";
            Status = 1;
        }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string ISD { get; set; }
        public string Mobile { get; set; }
        public string LLISD { get; set; }
        public string Landline { get; set; }
        public string UserRole { get; set; }
        public int Status { get; set; }
        public string Action { get; set; }
        public string Department { get; set; }
        public int DepartmentStatus { get; set; }
        public string Location { get; set; }
        public string LocationAddress { get; set; }
        public int LocationStatus { get; set; }
        public string Security { get; set; }
        public string PingMethods { get; set; }
        public string IncidentMethods { get; set; }
        public string SecurityDescription { get; set; }
        public string LocationAction { get; set; }
        public string DepartmentAction { get; set; }
    }
}
