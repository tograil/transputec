using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentById;

public class GetCompanyIncidentByIdRequest : IRequest<BaseResponse>
{
    public int CompanyId { get; set; }
    public int IncidentId { get; set; }
    public string UserStatus { get; set; }
}