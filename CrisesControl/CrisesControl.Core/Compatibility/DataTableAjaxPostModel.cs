using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrisesControl.Core.Compatibility;

[Obsolete("Added for compatibility with old portal")]
public class DataTableAjaxPostModel
{
    public DataTableAjaxPostModel()
    {
        Filters = string.Empty;
        UniqueKey = string.Empty;
    }
    public bool SkipDeleted { get; set; }
    public bool ActiveOnly { get; set; }
    public bool SkipInActive { get; set; }
    public bool KeyHolderOnly { get; set; }
    // properties are not capital due to json mapping
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public List<Column>? Columns { get; set; }
    [NotMapped]
    public Search? Search { get; set; }
    public List<Order>? Order { get; set; }
    public string UniqueKey { get; set; }
    public string Filters { get; set; }
}