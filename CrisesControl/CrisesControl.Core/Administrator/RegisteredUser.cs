using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class RegisteredUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public UserFullName RegisteredUserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        }
        [NotMapped]
        public PhoneNumber UserContactNumber
        {
            get { return new PhoneNumber { ISD = ISDCode, Number = Number }; }
            set { new PhoneNumber { ISD = ISDCode, Number = Number }; }
        }
        public string ISDCode { get; set; }
        public string Number { get; set; }
        public string UserEmailId { get; set; }
        public int UserStatus { get; set; }
        public string Password { get; set; }
        public string UniqueGuiID { get; set; }
    }
}
