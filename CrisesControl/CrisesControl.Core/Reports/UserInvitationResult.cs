using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class UserInvitationResult
    {
        public int UserId { get; set; }
        public UserFullName UserName
        {
            get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
            set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PhoneNumber UserContactNumber
        {
            get { return new PhoneNumber { ISD = ISDCode, Number = MobileNo }; }
            set { new PhoneNumber { ISD = ISDCode, Number = MobileNo }; }
        }
        public string ISDCode { get; set; }
        public string MobileNo { get; set; }
        public string PrimaryEmail { get; set; }
        public string InvitationSent { get; set; }
        public string CredentialSent { get; set; }
        public DateTimeOffset DateSent { get; set; }
    }

    public class UserInvitationModel : CcBase
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
    }
}
