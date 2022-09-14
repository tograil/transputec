using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Core.System
{
    public class TwilioLogModel
    {
        public string LogType { get; set; }
        [NotMapped]
        public List<CallResource> Calls { get; set; }
        [NotMapped]

        public List<MessageResource> Texts { get; set; }
        [NotMapped]

        public List<RecordingResource> Recordings { get; set; }
    }
}
