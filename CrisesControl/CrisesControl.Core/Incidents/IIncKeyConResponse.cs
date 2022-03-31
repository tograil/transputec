using CrisesControl.Core.Users;

namespace CrisesControl.Core.Incidents;

public class IIncKeyConResponse
{
    public int UserId { get; set; }
    public UserFullName UserName { get; set; }
    public string UserPhoto { get; set; }

    public string? FirstName
    {
        get => UserName?.Firstname ?? null;
        set
        {
            if (UserName.Firstname != value)
                UserName.Firstname = value!;
        }
    }
    public string? LastName
    {
        get => UserName?.Lastname ?? null;
        set
        {
            if (UserName.Lastname != value)
                UserName.Lastname = value;
        }
    }
}