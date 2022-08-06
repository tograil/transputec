using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CategoryTag
    {
        public int TagCategoryId { get; set; }
        public string TagCategoryName { get; set; }
        public string TagCategorySearchTerms { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
