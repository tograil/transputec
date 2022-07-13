using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompanyPackageItems
    {
        public int CompanyPackageItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string ItemValue { get; set; }
        public int Status { get; set; }
    }
}
