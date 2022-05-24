using System;

namespace CrisesControl.Core.Tasks.SP_Response;
public class GetCheckListReponseOption
{
    public int ResponseId { get; set; }
    public string Response { get; set; }
    public bool MarkDone { get; set; }
    public int CompanyId { get; set; }
    public int UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }

}