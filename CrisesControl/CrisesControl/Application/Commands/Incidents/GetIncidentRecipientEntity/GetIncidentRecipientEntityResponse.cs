using CrisesControl.Core.Messages;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentRecipientEntity
{
    public class GetIncidentRecipientEntityResponse
    {
        public List<MessageGroupObject> Data { get; set; }
        public string Message { get; set; }
    }
}
