using MediatR;
using CrisesControl.SharedKernel.Enums;
namespace CrisesControl.Api.Application.Commands.Messaging.GetConfUser
{
    public class GetConfUserRequest : IRequest<GetConfUserResponse>
    {
        public int ObjectId { get; set; }
        public MethodType ObjectType { get; set; }
    }
}
