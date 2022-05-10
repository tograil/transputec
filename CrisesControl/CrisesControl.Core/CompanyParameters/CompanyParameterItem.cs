using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters {
    public class CompanyParameterItem {
        public int CompanyparameterId { get; set; }
        public string ParameterName { get; set; }
        public string? ParameterValue { get; set; }
        public string? Display { get; set; }
        public string? Desc { get; set; }
        public string? ParameterType { get; set; }
        public int ParameterStatus { get; set; }
        public string? AllowedValued { get; set; }
        public string? ValidationRule { get; set; }
        public int Order { get; set; }
        public string? EditLevel { get; set; }
    }
}
