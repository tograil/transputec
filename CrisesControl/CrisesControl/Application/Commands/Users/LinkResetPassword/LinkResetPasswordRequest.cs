using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.LinkResetPassword
{
    public class LinkResetPasswordRequest:IRequest<LinkResetPasswordResponse>
    {
      
        public string QueriedGuid { get; set; }
        /// <summary>
        /// New password to change the old password of user 
        /// </summary>
        public string NewPassword { get; set; }
    }
}
