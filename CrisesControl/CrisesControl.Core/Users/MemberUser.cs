namespace CrisesControl.Core.Users;

public class MemberUser
{
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserFullName UserFullName { get; set; }
    public string UserPhoto { get; set; }
    public string UserRole { get; set; }
    public string UserEmail { get; set; }
    public string PrimaryEmail { get; set; }
    public int Status { get; set; }
    public bool ReceiveOnly { get; set; }
}