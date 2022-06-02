namespace CrisesControl.Core.Exceptions
{
    public interface IErrorData
    {
        public int CompanyId { get; }
        public int UserId { get; }
    }
}