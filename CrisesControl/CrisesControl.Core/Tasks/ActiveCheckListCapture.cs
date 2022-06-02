using CrisesControl.Core.Common;
using CrisesControl.Core.Compatibility;
using System.Collections.Generic;

namespace CrisesControl.Core.Tasks;
public class ActiveCheckListCapture : CcBase
{
    public int ActiveIncidentTaskId { get; set; }
    public List<CheckListOption> CheckListResponse { get; set; }
}