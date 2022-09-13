using Twilio.Rest.Verify.V2.Service;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioVerifyCheck
{
    public class TwilioVerifyCheckResponse
    {
        public VerificationCheckResource Data { get; set; }
    }
}
