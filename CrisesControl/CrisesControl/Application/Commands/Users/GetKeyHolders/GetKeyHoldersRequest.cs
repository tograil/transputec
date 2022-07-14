using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetKeyHolders
{
    public class GetKeyHoldersRequest : IRequest<GetKeyHoldersResponse>
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }
}
