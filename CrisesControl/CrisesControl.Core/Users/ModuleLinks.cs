using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class ModuleLinks
    {
        public string Module { get; set; }
        public string ModuleItem { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { }
        }

    }
}
