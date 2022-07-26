using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts
{
    public class GetFailedAttemptsRequest:IRequest<GetFailedAttemptsResponse>
    {
        public int MessageListID { get; set; }
        public string CommsMethod { get; set; }
    }
}
