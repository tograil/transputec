using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAddress
{
    public class GetAddressResponse
    {
        public Address data { get; set; }
        public string Message { get; set; }
    }
}
