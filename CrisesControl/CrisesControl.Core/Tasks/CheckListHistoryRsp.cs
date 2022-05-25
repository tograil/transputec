using CrisesControl.Core.Users;
using System;

namespace CrisesControl.Core.Tasks;
public class CheckListHistoryRsp
{
    public string Response { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool Done { get; set; }
    public UserFullName ActionBy
    {
        get { return new UserFullName { Firstname = FirstName, Lastname = LastName }; }
        set { new UserFullName { Firstname = FirstName, Lastname = LastName }; }
    }
    public string Comment { get; set; }
}