using CrisesControl.Core.Users;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class IncidentUserLocationResponse
    {
        public int UserId { get; set; }
        public string MessageLat { get; set; }
        public string MessageLng { get; set; }
        public UserFullName UserName { get; set; }
        public string UserEmail { get; set; }
        public PhoneNumber UserMobile { get; set; }
        public string UserPhoto { get; set; }
        public string UserFirstName
        {
            get
            {
                return UserName?.Firstname ?? null;
            }
            set
            {
                if (UserName == null)
                {
                    UserName = new Users.UserFullName();
                }
                if (UserName.Firstname != value)
                {
                    UserName.Firstname = value;
                }
            }
        }
        public string UserLastName
        {
            get
            {
                return UserName?.Lastname ?? null;
            }
            set
            {
                if (UserName == null)
                {
                    UserName = new Users.UserFullName();
                }
                if (UserName.Lastname != value)
                {
                    UserName.Lastname = value;
                }
            }
        }
        public string UserISDCode
        {
            get
            {
                return UserMobile?.ISD ?? null;
            }
            set
            {
                if (UserMobile == null)
                {
                    UserMobile = new Users.PhoneNumber();
                }
                if (UserMobile.ISD != value)
                {
                    UserMobile.ISD = value;
                }
            }
        }
        public string UserMobileNumber
        {
            get
            {
                return UserMobile?.Number ?? null;
            }
            set
            {
                if (UserMobile == null)
                {
                    UserMobile = new Users.PhoneNumber();
                }
                if (UserMobile.Number != value)
                {
                    UserMobile.Number = value;
                }
            }
        }
    }
}
