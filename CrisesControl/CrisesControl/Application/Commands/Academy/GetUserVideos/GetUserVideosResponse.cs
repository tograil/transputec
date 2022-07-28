using CrisesControl.Core.Academy;

namespace CrisesControl.Api.Application.Commands.Academy.GetUserVideos
{
    public class GetUserVideosResponse
    {
        public List<AcademyVideos> Data { get; set; }
        public string Message { get; set; }
    }
}
