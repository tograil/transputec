using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ForgotPassword
{
    public class ForgotPasswordRequest:IRequest<ForgotPasswordResponse>
    {
        public ForgotPasswordRequest()
        {
            Source = "APP";
            Method = MessageType.Email;
        }
        public string EmailId { get; set; }
        public string Source { get; set; }
        public MessageType Method { get; set; }
        public string Return { get; set; }
        public string CustomerId { get; set; }
        public string OTPMessage { get; set; }
    }
}
