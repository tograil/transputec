using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateLocation
{
    public class UpdateLocationRequest : IRequest<UpdateLocationResponse>
    {
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
        public int CompanyId { get; set; }
        public int Status { get; set; }
        public string? Desc { get; set; }
        public string? PostCode { get; set; }
    }
}
