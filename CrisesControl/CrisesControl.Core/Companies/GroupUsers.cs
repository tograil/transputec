using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies
{
    public class GroupUsers
    {
        public int UserId { get; set; }
        [NotMapped]
        public UserFullName UserFullName { get; set; }
        public string UserPhoto { get; set; }
        public string UserRole { get; set; }
        public int Status { get; set; }
    }
}
