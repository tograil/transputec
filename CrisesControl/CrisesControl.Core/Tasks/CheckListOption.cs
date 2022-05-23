namespace CrisesControl.Core.Tasks;
public class CheckListOption
{
    public int ActiveCheckListResponseId { get; set; }
    public int CheckListResponseId { get; set; }
    public int CheckListId { get; set; }
    public int ResponseId { get; set; }
    public string Response { get; set; }
    public int ActiveCheckListId { get; set; }
    public bool MarkDone { get; set; }
}