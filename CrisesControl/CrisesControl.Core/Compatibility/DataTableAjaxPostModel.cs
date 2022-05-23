using System;
using System.Collections.Generic;

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
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    public List<Column>? columns { get; set; }
    public Search? search { get; set; }
    public List<Order>? order { get; set; }
    public string UniqueKey { get; set; }
    public string Filters { get; set; }
}

[Obsolete("Added for compatibility with old portal")]
public class Column
{
    public string data { get; set; }
    public string name { get; set; }
    public bool searchable { get; set; }
    public bool orderable { get; set; }
    public Search search { get; set; }
}

[Obsolete("Added for compatibility with old portal")]
public class Search
{
    public string value { get; set; }
    public string regex { get; set; }
}

[Obsolete("Added for compatibility with old portal")]
public class Order
{
    public string column { get; set; }
    public string dir { get; set; }
}