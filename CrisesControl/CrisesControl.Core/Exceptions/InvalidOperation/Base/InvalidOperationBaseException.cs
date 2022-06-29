using System;

namespace CrisesControl.Core.Exceptions.InvalidOperation.Base
{
    public abstract class InvalidOperationBaseException : Exception
    {
        protected InvalidOperationBaseException(int companyId, int userId)
        {
            ErrorData = new ErrorData(companyId, userId);
        }

        public IErrorData ErrorData { get; }
    }
}