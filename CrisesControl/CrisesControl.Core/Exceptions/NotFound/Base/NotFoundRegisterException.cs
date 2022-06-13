using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Exceptions.NotFound.Base
{
    public abstract class NotFoundRegisterException :Exception
    {
        protected NotFoundRegisterException(int RegID, int userId)
        {
            ErrorData = new RegisterError(RegID, userId);
        }

        public IRegisterError ErrorData { get; }
    }
}
