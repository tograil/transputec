using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetGroupUsers
{
    public class GetGroupUsersRequest:IRequest<GetGroupUsersResponse>
    {
        public int GroupId { get; set; }
        public int ObjectMappingId { get; set; }
    }
}
