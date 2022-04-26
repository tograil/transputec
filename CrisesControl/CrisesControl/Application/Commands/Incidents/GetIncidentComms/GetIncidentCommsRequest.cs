using CrisesControl.Api.Application.Commands.Common;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentComms;

public class GetIncidentCommsRequest : IRequest<BaseResponse>
{
    public int ItemID { get; set; }
    public string Type { get; set; }
}