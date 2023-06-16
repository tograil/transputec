using System;

namespace CrisesControl.Core.Tasks;
public class CheckListResponseUpsert
{
    public int CheckListResponseId { get; set; }
    public int CheckListId { get; set; }
    public int ResponseId { get; set; }
    public string? Response { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public int UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
}