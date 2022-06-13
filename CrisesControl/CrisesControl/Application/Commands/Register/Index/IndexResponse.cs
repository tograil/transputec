using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Register.Index
{
    public class IndexResponse
    {
        public List<Registration> Data { get; set; }
        public string Message { get; set; }
    }
}
