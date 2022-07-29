using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempUser
{
    public class GetTempUserRequest:IRequest<GetTempUserResponse>
    {
        public int TempUserId { get; set; }
    }
}
