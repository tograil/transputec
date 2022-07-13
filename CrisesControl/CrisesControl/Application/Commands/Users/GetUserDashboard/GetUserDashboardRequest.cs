using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUserDashboard
{
    public class GetUserDashboardRequest : IRequest<GetUserDashboardResponse>
    {
        public string ModulePage { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }
        public bool Reverse { get; set; }
        public decimal XPos { get; set; }
        public decimal YPos { get; set; }
    }
}
