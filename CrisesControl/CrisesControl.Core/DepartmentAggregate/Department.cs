using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace CrisesControl.Core.DepartmentAggregate
{
    public record Department(string Name    ): BaseEntity, IAggregateRoot
    {
    }
}
