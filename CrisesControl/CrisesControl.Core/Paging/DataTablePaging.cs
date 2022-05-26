namespace CrisesControl.Core.Paging;

public class DataTablePaging
{
    public int draw { get; set; }
    public int recordsTotal { get; set; }
    public int recordsFiltered { get; set; }
    public object data { get; set; }
}