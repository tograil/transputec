using MediatR;

namespace CrisesControl.Api.Application.Commands.GetStarted
{
    public class GetStartedRequest:IRequest<GetStartedResponse>
    {
        public int CompanyId { get; set; }
    }
}
