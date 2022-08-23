using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.GetSopSections
{
    public class GetSopSectionsRequest:IRequest<GetSopSectionsResponse>
    {
        public int SOPHeaderID { get; set; }
    }
}
