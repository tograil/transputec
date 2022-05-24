using System.Collections.Generic;

namespace CrisesControl.Core.Tasks;
public class CheckList
{
    public int CheckListId { get; set; }
    public int TaskId { get; set; }
    public string? Description { get; set; }
    public bool DoneOnly { get; set; }
    public int OptionCount { get; set; }
    public int SortOrder { get; set; }
    public List<CheckListOption>? CheckListOptions { get; set; }
}