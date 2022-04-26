using CrisesControl.Api.Application.Commands.Common;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentComms;

public class GetIncidentCommsHandler
    : IRequestHandler<GetIncidentCommsRequest, BaseResponse>
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentCommsHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<BaseResponse> Handle(GetIncidentCommsRequest request, CancellationToken cancellationToken)
    {
        var result = _incidentRepository.GetIncidentComms(request.ItemID, request.Type);

        if (result != null)
        {
            return new BaseResponse()
            {
                Data = result,
                ErrorCode = "0"
            };
        }
        else
        {
            return new BaseResponse()
            {
                Data = result,
                ErrorCode = "110",
                Message = "No record found."
            };
        };
    }
}