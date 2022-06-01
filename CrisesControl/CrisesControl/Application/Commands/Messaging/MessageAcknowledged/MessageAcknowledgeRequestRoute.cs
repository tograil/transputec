namespace CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged
{
    public class MessageAcknowledgeRequestRoute
    {
        public int CompanyId { get; set; }
        public int CurrentUserId { get; set; }
        public int MsgListId { get; set; }
        public int ResponseID { get; set; }


    }
}
