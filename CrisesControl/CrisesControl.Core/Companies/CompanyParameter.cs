using System;

namespace CrisesControl.Core.Companies
{
    public partial class CompanyParameter
    {
        public int CompanyParametersId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
