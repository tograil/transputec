using CrisesControl.Core.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Incidents;

public class IncKeyCons
{
    public int UserId { get; set; }
    [NotMapped]
    public UserFullName UserName
    {
        get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        set { }
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}