using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Tasks;
public class CheckListUpsert
{
    public int CheckListId { get; set; }
    public int TempCheckListId { get; set; }
    public int TaskID { get; set; }
    public string Description { get; set; }
    public bool DoneOnly { get; set; }
    public int OptionCount { get; set; }
    public int SortOrder { get; set; }
    public int UpdatedBy { get; set; }
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    [NotMapped]
    public List<CheckListOption> CheckListOptions { get; set; }
}