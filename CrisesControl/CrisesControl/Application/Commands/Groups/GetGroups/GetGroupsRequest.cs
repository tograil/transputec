using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroups
{
    public class GetGroupsRequest : IRequest<GetGroupsResponse>
    {
        public int CompanyId { get; set; }
    }
}
