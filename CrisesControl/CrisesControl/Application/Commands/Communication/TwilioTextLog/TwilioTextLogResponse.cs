using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioTextLog
{
    public class TwilioTextLogResponse
    {
        public MessageResource Data { get; set; }
    }
}
