using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Addresses.GetAllAddress
{
    public class GetAllAddressResponse
    {
        public List<Address> Data { get; set; }
        public string Message { get; set; }
    }
}
