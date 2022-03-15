using MediatR;

namespace CrisesControl.Api.Application.Commands.Locations.UpdateLocation
{
    public class UpdateLocationRequest : IRequest<UpdateLocationResponse>
    {
        public int DepartmentId { get; set; }  
        public int CompanyId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
