using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetConfRecordings
{
    public class GetConfRecordingsRequest : IRequest<GetConfRecordingsResponse>
    {
        public int ConfCallId { get; set; }
        public int ObjectID { get; set; }
        public string ObjectType { get; set; }
        public bool Single { get; set; }
        public int OutUserCompanyId { get; set; }
    }
}
