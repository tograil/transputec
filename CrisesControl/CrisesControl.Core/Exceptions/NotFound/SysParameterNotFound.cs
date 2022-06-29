using CrisesControl.Core.Exceptions.NotFound.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.NotFound
{
    public class SysParameterNotFound : NotFoundRegisterException
    {
        public SysParameterNotFound(int companyId, int userId)
            : base(companyId, userId)
        {

        }

        public override string Message => "System Parameter not found";
    }
}
