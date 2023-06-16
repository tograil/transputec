using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates
{
    public class GetImportTemplatesResponse
    {
        public List<ImportTemplate> Data { get; set; }
        public string Message { get; set; }
    }
}
