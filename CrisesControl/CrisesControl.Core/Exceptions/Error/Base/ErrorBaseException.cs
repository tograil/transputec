using System;

namespace CrisesControl.Core.Exceptions.Error.Base
{
    public abstract class ErrorBaseException : Exception
    {
        public ErrorBaseException(int companyId, int userId)
        {
            ErrorData = new ErrorData(companyId, userId);
        }

        public IErrorData ErrorData { get; }
    }
}