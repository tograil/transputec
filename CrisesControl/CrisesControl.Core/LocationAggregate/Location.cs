using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate
{
    public record Location: IAggregateRoot
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
        public string Desc { get; set; }
        public string PostCode { get; set; }
    }
}
