using CrisesControl.Core.Companies;
using CrisesControl.Core.CompanyParameters;
using MediatR;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.SaveCascading
{
    public class SaveCascadingRequest:IRequest<SaveCascadingResponse>
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public bool LaunchSOS { get; set; }
        public int LaunchSOSInterval { get; set; }
        public string ParamName { get; set; }
        public bool EnableSetting { get; set; }
        public List<CommsMethodPriority> CommsMethod { get; set; }
        public string Type { get; set; }
        public PriorityLevel PingPriority { get; set; }
        public PriorityLevel IncidentPriority { get; set; }
        public SeverityLevel IncidentSeverity { get; set; }
    }
}
