using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class EntityRcpntResponse
    {
        public int UserId { get; set; }
        [NotMapped]
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public PhoneNumber UserContactNumber
        {
            get { return new PhoneNumber { ISD = ISDCode, Number = MobileNo }; }
            set { new PhoneNumber { ISD = ISDCode, Number = MobileNo }; }
        }
        public string ISDCode { get; set; }
        public string MobileNo { get; set; }
        public string PrimaryEmail { get; set; }
    }
}
