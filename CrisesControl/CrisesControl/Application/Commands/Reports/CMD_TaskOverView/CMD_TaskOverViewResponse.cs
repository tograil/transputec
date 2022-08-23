namespace CrisesControl.Api.Application.Commands.Reports.CMD_TaskOverView
{
    public class CMD_TaskOverViewResponse
    {
        public int Accepted { get; set; }
        public int NotAccepted { get; set; }
        public int PendingComplete { get; set; }
        public int Completed { get; set; }
        public int PendingPredecessor { get; set; }
        public int Total { get; set; }
    }
}
