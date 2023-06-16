using CrisesControl.Core.Users;

namespace CrisesControl.Core.Support
{
    public class KeyContacts
    {
        public int UserId { get; set; }
        public UserFullName KeyContactName { get; set; }
        public string KeyContactImage { get; set; }
        public string KeyContactEmail { get; set; }
        public PhoneNumber KeyContactMob { get; set; }
        public string KeyLat { get; set; }
        public string KeyLng { get; set; }
    }
}
