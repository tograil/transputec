using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate
{
    public record class Location(string Name) : BaseEntity, IAggregateRoot
    {
    }
}
