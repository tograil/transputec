using CrisesControl.Core.Incidents;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentType;

public class GetCompanyIncidentTypeRequest : IRequest<GetCompanyIncidentTypeResponse>
{
    public int CompanyId { get; set; }

    //public int CompanyId { get; set; }

    //public int CurrentUserId { get; set; }

    //public string Password { get; set; }
    //public string CompanyKey { get; set; }
    //public int CustomerId { get; set; }
    //public DateTimeOffset StartDate { get; set; }
    //public DateTimeOffset EndDate { get; set; }
    //public int SalesSource { get; set; }
    //public bool FilterVirtual { get; set; }
    //public string TOKEN { get; set; }
    //public int IncidentId { get; set; }
    //public int QUserId { get; set; }
}