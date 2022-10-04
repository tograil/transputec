using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetReplies
{
    public class GetRepliesRequest : IRequest<GetRepliesResponse>
    {
        public int ParentId { get; set; }
      
    }
}
