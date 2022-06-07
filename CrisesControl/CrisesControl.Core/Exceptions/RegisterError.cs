using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions
{
    public class RegisterError : IRegisterError
    {
        public RegisterError(int regId, int userId)
        {
            RegId = regId;
            UserId = userId;
        }

        public int RegId { get; }
        public int UserId { get; }
    }
}
