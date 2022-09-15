using CrisesControl.Core.App;

namespace CrisesControl.Api.Application.Commands.App.GetAppLanguage
{
    public class GetAppLanguageResponse
    {
        public List<AppLanguageList> Result { get; set; }
        public string Message { get; set; }
    }
}
