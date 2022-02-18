using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.LocationAggregate.Handlers.GetLocation
{
    public class GetLocationValidator : AbstractValidator<GetLocationRequest>
    {
        public GetLocationValidator()
        {
            RuleFor(x => x.LocationId)
                .GreaterThan(0);
        }
    }
}
