using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.ReplyToMessage
{
    public class ReplyToMessageRequest : IRequest<ReplyToMessageResponse>
    {
        public int ParentID { get; set; }
        public string MessageText { get; set; }
        public string ReplyTo { get; set; }
        public string MessageType { get; set; }
        public int ActiveIncidentID { get; set; }
        public int[] MessageMethod { get; set; }
        public int CascadePlanID { get; set; }
        public int CurrentUserId { get; set; }
        public int CompanyId { get; set; }
        public string TimeZoneId { get; set; }
    }
}
