using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class EmailTemplateList
    {
        public int TemplateID { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlData { get; set; }
        public int Status { get; set; }
        public string EmailSubject { get; set; }
        public int CompanyCopy { get; set; }
        public string Locale { get; set; }
    }
}
