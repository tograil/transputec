using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SaveMessageResponse
{
    public class SaveMessageResponseRequest : IRequest<SaveMessageResponseResponse>
    {
        public int ResponseId { get; set; }
        public string ResponseLabel { get; set; }
        public string Description { get; set; }
        public bool IsSafetyResponse { get; set; }
        public string SafetyAckAction { get; set; }
        public string MessageType { get; set; }
        public int Status { get; set; }
        //public int CurrentUserId { get; set; }
        //public int CompanyId { get; set; }
        //public string TimeZoneId { get; set; }
    }
}
