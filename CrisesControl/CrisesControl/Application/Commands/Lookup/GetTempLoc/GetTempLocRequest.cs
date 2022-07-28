using MediatR;

namespace CrisesControl.Api.Application.Commands.Lookup.GetTempLoc
{
    public class GetTempLocRequest:IRequest<GetTempLocResponse>
    {
        public int TempLocationId { get; set; }
    }
}
