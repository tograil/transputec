using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Addresses.AddAddress
{
    public class AddAddressRequest:IRequest<AddAddressResponse>
    {
        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string Postcode { get; set; } = null!;
        public string? CountryCode { get; set; }
        public AddressType AddressType { get; set; } 
        public string? AddressLabel { get; set; }
    }
}
