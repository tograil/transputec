using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveLanguageItem
{
    public class SaveLanguageItemRequest :IRequest<SaveLanguageItemResponse>
    {
        public int LanguageItemID { get; set; }
        public string LangKey { get; set; }
        public string LangValue { get; set; }
        public string ErrorCode { get; set; }
        public string Title { get; set; }
        public string Options { get; set; }
        public string ObjectType { get; set; }
        public string Locale { get; set; }
    }
}
