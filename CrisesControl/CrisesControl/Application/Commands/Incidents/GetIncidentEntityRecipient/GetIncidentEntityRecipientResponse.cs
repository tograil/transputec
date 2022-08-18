using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentEntityRecipient
{
    public class GetIncidentEntityRecipientResponse
    {
        public DataTablePaging DataTable { get; set; }
        public string Message { get; set; }
    }
}
