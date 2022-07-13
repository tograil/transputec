using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage
{
    public class GetAppLanguageResponse
    {
        public AppLanguages Data { get; set; }
        public string Message { get; set; }
    }
}
