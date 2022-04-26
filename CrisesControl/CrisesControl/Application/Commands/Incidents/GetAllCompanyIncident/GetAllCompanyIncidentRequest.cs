using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllCompanyIncident;

public class GetAllCompanyIncidentRequest : IRequest<BaseResponse>
{
    public int QUserId { get; set; }
}