namespace CrisesControl.Api.Application.Helpers;

public interface ICurrentUser
{
    int UserId { get; }
    string UserName { get; }
    int CompanyId { get; }
}