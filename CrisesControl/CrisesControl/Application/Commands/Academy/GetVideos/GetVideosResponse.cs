using CrisesControl.Core.Academy;

namespace CrisesControl.Api.Application.Commands.Academy.GetVideos
{
    public class GetVideosResponse
    {
        public List<AcademyVideos> Data { get; set; }
        public string Message { get; set; }
    }
}
