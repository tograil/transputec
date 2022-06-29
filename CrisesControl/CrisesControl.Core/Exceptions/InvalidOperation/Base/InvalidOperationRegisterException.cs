using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.InvalidOperation.Base
{
    public abstract class InvalidOperationRegisterException :Exception
    {
        protected InvalidOperationRegisterException(int RegID, int userId)
        {
            ErrorData = new RegisterError(RegID, userId);
        }

        public IRegisterError ErrorData { get; }
    }
}
