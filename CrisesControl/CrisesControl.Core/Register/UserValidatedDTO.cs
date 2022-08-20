using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Register
{
    public class UserValidatedDTO
    {
        public int companyid { get; set; }
        public int userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public int ErrorId { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string CustomerId { get; set; }
    }
}
