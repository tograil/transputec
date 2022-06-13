using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList
{
    public class GetMessageGroupListRequest: IRequest<GetMessageGroupListResponse>
    {
        public int MessageID { get; set; }
    }
}
