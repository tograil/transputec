using CrisesControl.Core.Common;
using System.Collections.Generic;

namespace CrisesControl.Core.Tasks;
public class ActiveCheckListCapture : CCBase
{
    public int ActiveIncidentTaskId { get; set; }
    public List<CheckListOption> CheckListResponse { get; set; }
}