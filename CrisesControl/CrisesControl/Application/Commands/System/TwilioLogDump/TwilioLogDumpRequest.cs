using CrisesControl.Core.System;
using CrisesControl.SharedKernel.Enums;
using MediatR;
using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Api.Application.Commands.System.TwilioLogDump
{
    public class TwilioLogDumpRequest : IRequest<TwilioLogDumpResponse>
    {
        public LogType LogType { get; set; }
    
        public List<CallResource> Calls { get; set; }       

        public List<MessageResource> Texts { get; set; }       

        public List<RecordingResource> Recordings { get; set; }
    }
}
