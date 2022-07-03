using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.AddDashlet
{
    public class AddDashletRequest : IRequest<AddDashletResponse>
    {
        public int ModuleId { get; set; }
        public int UserId { get; set; }
        public decimal XPos { get; set; }
        public decimal YPos { get; set; }
    }
}
