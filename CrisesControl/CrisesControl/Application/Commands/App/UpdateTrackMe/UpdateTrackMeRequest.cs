using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdateTrackMe
{
    public class UpdateTrackMeRequest:IRequest<UpdateTrackMeResponse>
    {
        public bool Enabled { get; set; }
        public string TrackType { get; set; }
        public int ActiveIncidentID { get; set; }
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude of the current location of the user, collected by the GPS tracker, send 0 in case GPS is not enabled
        /// </summary>
        public string Longitude { get; set; }
        public int UserDeviceID { get; set; }
        public int UserId { get; set; }
    }
}
