using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SendPasswordOTP
{
    public class SendPasswordOTPRequest : IRequest<SendPasswordOTPResponse>
    {
        public SendPasswordOTPRequest()
        {
            Return = "bool";
            OTPMessage = "";
            Source = "RESET";
            Method = "TEXT";
        }
        public string Action { get; set; }
        public int UserID { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string OTPCode { get; set; }
        public string Return { get; set; }
        public string OTPMessage { get; set; }
        public string Source { get; set; }
        public string Method { get; set; }
    }
}
