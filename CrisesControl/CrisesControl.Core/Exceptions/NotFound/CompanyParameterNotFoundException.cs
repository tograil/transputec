using CrisesControl.Core.Exceptions.NotFound.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.NotFound
{
    public class CompanyParameterNotFoundException : NotFoundRegisterException
    {
        public CompanyParameterNotFoundException(int companyId, int userId)
            : base(companyId, userId)
        {

        }

        public override string Message => "Company Parameter not found";
    }
}
