using Twilio.Base;
using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioText
{
    public class TwilioTextResponse
    {
        public ResourceSet<RecordingResource> Data { get; set; }
    }
}
