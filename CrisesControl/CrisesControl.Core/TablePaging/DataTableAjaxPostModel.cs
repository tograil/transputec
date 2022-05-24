using System.Collections.Generic;

namespace CrisesControl.Core.TablePaging;
public class DataTableAjaxPostModel
{
    public DataTableAjaxPostModel()
    {
        Filters = "";
        UniqueKey = "";
    }
    public bool SkipDeleted { get; set; }
    public bool ActiveOnly { get; set; }
    public bool SkipInActive { get; set; }
    public bool KeyHolderOnly { get; set; }
    // properties are not capital due to json mapping
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    public List<Column> columns { get; set; }
    public Search search { get; set; }
    public List<Order> order { get; set; }
    public string UniqueKey { get; set; }
    public string Filters { get; set; }
}