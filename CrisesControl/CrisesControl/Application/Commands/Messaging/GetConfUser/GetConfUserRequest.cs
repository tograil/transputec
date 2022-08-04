using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetConfUser
{
    public class GetConfUserRequest : IRequest<GetConfUserResponse>
    {
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
