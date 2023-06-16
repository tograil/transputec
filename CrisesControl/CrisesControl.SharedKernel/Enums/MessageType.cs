using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.SharedKernel.Enums;

public enum MessageType
{
    Email,
    Phone ,
    Push,
    Text,
}
public enum MessageCheckType
{
    Ping,
    Incident
}
