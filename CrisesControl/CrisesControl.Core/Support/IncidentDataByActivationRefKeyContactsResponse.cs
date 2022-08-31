using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Support
{
    public class IncidentDataByActivationRefKeyContactsResponse : KeyContacts
    {
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
                    KeyContactName = new Users.UserFullName();
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
                    KeyContactName = new Users.UserFullName();
                }
                if (KeyContactName.Lastname != value)
                {
                    KeyContactName.Lastname = value;
                }
            }
        }
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
                    KeyContactMob = new Users.PhoneNumber();
                }
                if (KeyContactMob.ISD != value)
                {
                    KeyContactMob.ISD = value;
                }
            }
        }
        public string KeyContactMobileNo
        {
            get
            {
                return KeyContactMob?.Number ?? null;
            }
            set
            {
                if (KeyContactMob == null)
                {
                    KeyContactMob = new Users.PhoneNumber();
                }
                if (KeyContactMob.Number != value)
                {
                    KeyContactMob.Number = value;
                }
            }
        }

        public KeyContacts ToKeyContacts() => new KeyContacts
        {
            KeyContactEmail = KeyContactEmail,
            KeyContactImage = KeyContactImage,
            KeyContactMob = KeyContactMob,
            KeyContactName = KeyContactName,
            KeyLat = KeyLat,
            KeyLng = KeyLng,
            UserId = UserId
        };
    }
}
