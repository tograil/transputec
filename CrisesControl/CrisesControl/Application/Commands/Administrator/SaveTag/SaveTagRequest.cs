using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveTag
{
    public class SaveTagRequest:IRequest<SaveTagResponse>
    {
        public int TagID { get; set; }
        public int TagCategoryID { get; set; }
        public string TagName { get; set; }
        public string SearchTerms { get; set; }
    }
}
