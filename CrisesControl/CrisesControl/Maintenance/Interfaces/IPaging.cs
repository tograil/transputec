namespace CrisesControl.Api.Maintenance.Interfaces
{
    public interface IPaging
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool Apply { get; set; }
    }
}