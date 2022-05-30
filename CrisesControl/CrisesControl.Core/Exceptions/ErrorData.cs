namespace CrisesControl.Core.Exceptions
{
    internal class ErrorData : IErrorData
    {
        public ErrorData(int companyId, int userId)
        {
            CompanyId = companyId;
            UserId = userId;
        }

        public int CompanyId { get; }
        public int UserId { get; }
    }
}