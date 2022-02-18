using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.GroupAggregate.Handlers.GetGroup
{
    public class GetGroupValidator : AbstractValidator<GetGroupRequest>
    {
        public GetGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0);
        }
    }
}
