using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentEntityRecipient
{
    public class GetIncidentEntityRecipientRequest:IRequest<GetIncidentEntityRecipientResponse>
    {
        public int ActiveIncidentID { get; set; }
        public string EntityType { get; set; }
        public int EntityID { get; set; }
        //public Search Search { get; set; }
        //public int Draw { get; set; }
        //public string Dir { get; set; }
        //public string CompanyKey { get; set; }

    }
}
