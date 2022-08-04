using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveTagCategory
{
    public class SaveTagCategoryRequest:IRequest<SaveTagCategoryResponse>
    {
        public int TagCategoryID { get; set; }
        [MaxLength(100)]
        public string TagCategoryName { get; set; }
        [MaxLength(250)]
        public string TagCategorySearchTerms { get; set; }
    }
}
