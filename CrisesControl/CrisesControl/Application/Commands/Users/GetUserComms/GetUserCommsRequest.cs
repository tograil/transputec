using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserComms
{
    public class GetUserCommsRequest : IRequest<GetUserCommsResponse>
    {
        public int CommsUserId { get; set; }  
    }
}
