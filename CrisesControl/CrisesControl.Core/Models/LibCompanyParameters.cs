using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Models
{
    public partial class LibCompanyParameters
    {
        public int LibCompanyParametersId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string AllowedValued { get; set; }
        public string ValidationRule { get; set; }
        public int Order { get; set; }
        public string EditLevel { get; set; }
    }
}
