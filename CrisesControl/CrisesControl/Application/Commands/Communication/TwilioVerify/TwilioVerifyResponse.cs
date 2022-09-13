using Twilio.Rest.Verify.V2.Service;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerify
{
    public class TwilioVerifyResponse
    {
        public VerificationResource Data { get; set; }
    }
}
