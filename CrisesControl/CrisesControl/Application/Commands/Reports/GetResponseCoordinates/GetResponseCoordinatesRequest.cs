using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetResponseCoordinates
{
    public class GetResponseCoordinatesRequest:IRequest<GetResponseCoordinatesResponse>
    {
        public int MessageId { get; set; }
    }
}
