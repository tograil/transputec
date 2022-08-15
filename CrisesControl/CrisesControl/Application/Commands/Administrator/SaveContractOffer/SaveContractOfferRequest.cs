using CrisesControl.Core.Administrator;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SaveContractOffer
{
    public class SaveContractOfferRequest:IRequest<SaveContractOfferResponse>
    {
        public PreContractOfferModel IP { get; set; }
    }
}
