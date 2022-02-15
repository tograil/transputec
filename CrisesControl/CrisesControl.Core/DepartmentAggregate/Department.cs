using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace CrisesControl.Core.DepartmentAggregate
{
    public record Department(string Name, List<string> Pings): BaseEntity, IAggregateRoot
    {
        public void AddPing(string empty)
        {
            Pings.Add(empty);
        }
    }
}
