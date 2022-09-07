using CrisesControl.Core.App;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.App.GetLanguage
{
    public class GetLanguageResponse
    {
        public List<LanguageItem> Result { get; set; }
        public string Message { get; set; }
    }
}
