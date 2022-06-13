using CrisesControl.Core.Exceptions.NotFound.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.NotFound
{
    public class RegisterNotFoundException: NotFoundRegisterException
    {
        public RegisterNotFoundException(int regId, int userId) : base(regId,userId)
        {

        }
        public override string Message => "Temp Registered not found";
    }
}
