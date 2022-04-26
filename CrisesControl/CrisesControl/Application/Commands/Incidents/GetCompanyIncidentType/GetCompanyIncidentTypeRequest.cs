using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanyIncidentType;

public class GetCompanyIncidentTypeRequest : IRequest<BaseResponse>
{
    public int CompanyId { get; set; }
}