namespace CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount {
    public class GetNotificationsCountResponse {
        public int UserId { get; set; }
        public int TotalIncidentUnAck { get; set; }
        public int TotalPingUnAck { get; set; }
        public int TaskCount { get; set; }
        public int PendingTask { get; set; }
        public int ActiveCompletedTask { get; set; }
        public int ActiveOffDuty { get; set; }
    }
}
