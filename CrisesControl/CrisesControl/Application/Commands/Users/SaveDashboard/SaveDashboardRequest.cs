using CrisesControl.Core.Models;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SaveDashboard
{
    public class SaveDashboardRequest : IRequest<SaveDashboardResponse>
    {
        public List<DashboardModule> ModuleItems { get; set; }
        public string ModulePage { get; set; }
        public int ModuleID { get; set; }
        public int UserID { get; set; }
        public bool Reverse { get; set; }
        public decimal XPos { get; set; }
        public decimal YPos { get; set; }
    }
}
