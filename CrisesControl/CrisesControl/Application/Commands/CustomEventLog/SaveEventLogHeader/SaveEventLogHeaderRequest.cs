using MediatR;

namespace CrisesControl.Api.Application.Commands.CustomEventLog.SaveEventLogHeader
{
    public class SaveEventLogHeaderRequest : IRequest<SaveEventLogHeaderResponse>
    {
        public int ActiveIncidentId { get; set; }
        public int PermittedDepartment { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }

    }
}
