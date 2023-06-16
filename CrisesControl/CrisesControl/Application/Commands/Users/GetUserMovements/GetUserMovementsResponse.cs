using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Users.GetUserMovements
{
    public class GetUserMovementsResponse
    {
        public List<UserLocation> Locations { get; set; }
    }
}
