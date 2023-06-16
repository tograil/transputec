namespace CC.Authority.Implementation.Helpers
{
    public interface ICurrentUser
    {
        int UserId { get; }
        string UserName { get; }
        int CompanyId { get; }
        string TimeZone { get; }
    }
}