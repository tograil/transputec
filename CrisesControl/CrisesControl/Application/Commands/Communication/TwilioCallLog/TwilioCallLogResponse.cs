using CrisesControl.Core.Register;
using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Api.Application.Commands.Communication.TwilioCallLog
{
    public class TwilioCallLogResponse
    {
        public CallResource Data { get; set; }
    }
}
