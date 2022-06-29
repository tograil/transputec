using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class IncidentReportKeyContactResponse
    {
        public int UserId { get; set; }
        public string KeyContactFirstName
        {
            get
            {
                return KeyContactName?.Firstname ?? null;
            }
            set
            {
                if (KeyContactName == null)
                {
                    KeyContactName = new UserFullName();
                }
                if (KeyContactName.Firstname != value)
                {
                    KeyContactName.Firstname = value;
                }
            }
        }
        public string KeyContactLastName
        {
            get
            {
                return KeyContactName?.Lastname ?? null;
            }
            set
            {
                if (KeyContactName == null)
                {
                    KeyContactName = new UserFullName();
                }
                if (KeyContactName.Lastname != value)
                {
                    KeyContactName.Lastname = value;
                }
            }
        }
        [NotMapped]
        public UserFullName KeyContactName { get; set; }
        public string KeyContactImage { get; set; }
        public string KeyContactEmail { get; set; }
        public string KeyContactISDCode
        {
            get
            {
                return KeyContactMob?.ISD ?? null;
            }
            set
            {
                if (KeyContactMob == null)
                {
                    KeyContactMob = new PhoneNumber();
                }
                if (KeyContactMob.ISD != value)
                {
                    KeyContactMob.ISD = value;
                }
            }
        }
        public string KeyContactMobile
        {
            get
            {
                return KeyContactMob?.Number ?? null;
            }
            set
            {
                if (KeyContactMob == null)
                {
                    KeyContactMob = new PhoneNumber();
                }
                if (KeyContactMob.Number != value)
                {
                    KeyContactMob.Number = value;
                }
            }
        }
        [NotMapped]
        public PhoneNumber KeyContactMob { get; set; }
    }
}
