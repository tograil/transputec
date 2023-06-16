using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CheckUserLicense
{
    
    public class CheckUserLicenseRequest : IRequest<CheckUserLicenseResponse>
    {
        public List<UserRoles> UserList { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }

}
