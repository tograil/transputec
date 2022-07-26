using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class ResponseCordinates
    {
        public int UserId { get; set; }
        public string MessageLat { get; set; }
        public string MessageLng { get; set; }
        [NotMapped]
        public PhoneNumber UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoto { get; set; }
        [NotMapped]
        public UserFullName UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ISDCode { get; set; }
        public string MobileNo { get; set; }

    }
}
