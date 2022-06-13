namespace CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList
{
    public class GetAllOneUserDeviceListRequest
    {
        public int QueriedUserId { get; set; }
        public string Filters { get; set; }
    }
}
