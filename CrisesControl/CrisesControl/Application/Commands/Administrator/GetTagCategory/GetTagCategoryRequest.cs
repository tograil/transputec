using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTagCategory
{
    public class GetTagCategoryRequest:IRequest<GetTagCategoryResponse>
    {
        public int TagCategoryID { get; set; }
    }
}
