using System.Collections.Generic;
using CrisesControl.SharedKernel;
using CrisesControl.SharedKernel.Interfaces;

namespace CrisesControl.Core.CompanyAggregate
{
    public record Company(string Name, List<string> Pings) : BaseEntity, IAggregateRoot
    {
        public void AddPing(string empty)
        {
            Pings.Add(empty);
        }
    }
}