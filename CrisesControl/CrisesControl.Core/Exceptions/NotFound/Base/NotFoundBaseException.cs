using System;

namespace CrisesControl.Core.Exceptions.NotFound.Base
{
    public abstract class NotFoundBaseException : Exception
    {
        protected NotFoundBaseException(int companyId, int userId)
        {
            ErrorData = new ErrorData(companyId, userId);
        }

        public IErrorData ErrorData { get; }
    }
}