﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.GroupAggregate.Handlers.GetGroup
{
    public class GetGroupRequest : IRequest<GetGroupResponse>
    {
        public int GroupId { get; set; }
    }
}
