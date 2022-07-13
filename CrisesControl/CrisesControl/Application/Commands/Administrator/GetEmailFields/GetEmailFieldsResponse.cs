using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetEmailFields
{
    public class GetEmailFieldsResponse
    {
        public List<EmailFieldLookup> Data { get; set; }
        public string Message { get; set; }
    }
}
