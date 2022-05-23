using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class GetAllUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileISDCode { get; set; }
        public string MobileNo { get; set; }
        public string LLISDCode { get; set; }
        public string Landline { get; set; }
        public string PrimaryEmail { get; set; }
        public string UserPhoto { get; set; }
        public string UserRole { get; set; }
        public string UniqueGuiID { get; set; }
        public bool RegisterUser { get; set; }
        public int Status { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public string DepartmentName { get; set; }
    }
}
