using MediatR;

namespace CrisesControl.Api.Application.Commands.App.GetLanguage
{
    public class GetLanguageRequest:IRequest<GetLanguageResponse>
    {
        public string Locale { get; set; }
    }
}
