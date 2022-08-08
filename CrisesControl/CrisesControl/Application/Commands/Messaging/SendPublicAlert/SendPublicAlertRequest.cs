using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.SendPublicAlert
{
    public class SendPublicAlertRequest : IRequest<SendPublicAlertResponse>
    {
        public string MessageText { get; set; }
        public int[] MessageMethod { get; set; }
        public bool SchedulePA {get;set;}
        public DateTime ScheduleAt {get;set;}
        public string SessionId {get;set;}
        public int UserID {get;set;}
        public int CompanyID {get;set;}
        public string TimeZoneId {get;set;}
    }
}
