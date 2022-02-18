using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate.Handlers.GetLocation
{
    public class GetLocationRequest : IRequest<GetLocationResponse>
    {
        public int LocationId { get; set; }
    }
}
