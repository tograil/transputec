using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CheckUserLicense
{
    public class UserRoles
    {
        public int UserId { get; set; }
        public string UserRole { get; set; }
    }
    public class CheckUserLicenseRequest : IRequest<CheckUserLicenseResponse>
    {
        public List<UserRoles> UserList { get; set; }
        public string SessionId { get; set; }
    }

}
