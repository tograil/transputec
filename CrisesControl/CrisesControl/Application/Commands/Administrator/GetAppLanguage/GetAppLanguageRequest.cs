using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAppLanguage
{
    public class GetAppLanguageRequest:IRequest<GetAppLanguageResponse>
    {
        public string LangKey { get; set; }
        public string Locale { get; set; }
        public int LanguageItemID { get; set; }
        public string ObjectType { get; set; }
    }
}
