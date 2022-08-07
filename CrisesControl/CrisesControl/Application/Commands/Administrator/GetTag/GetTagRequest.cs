using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTag
{
    public class GetTagRequest:IRequest<GetTagResponse>
    {
        public int TagId { get; set; }
    }
}
