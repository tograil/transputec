using CrisesControl.Api.Application.Commands.Addresses.AddAddress;
using CrisesControl.Api.Application.Commands.Addresses.DeleteAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAllAddress;
using CrisesControl.Api.Application.Commands.Addresses.UpdateAddress;

namespace CrisesControl.Api.Application.Query
{
    public interface IAddressQuery
    {
        Task<GetAllAddressResponse> GetAllAddress(GetAllAddressRequest request);
        Task<AddAddressResponse> AddAddress(AddAddressRequest request);
        Task<UpdateAddressResponse> UpdateAddress(UpdateAddressRequest request);
        Task<DeleteAddressResponse> DeleteAddress(DeleteAddressRequest request);
        Task<GetAddressResponse> GetAdress(GetAddressRequest request);
    }
}
