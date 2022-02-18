using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.GroupAggregate
{
    public record class Group(string Name) : BaseEntity, IAggregateRoot
    {
    }
}
