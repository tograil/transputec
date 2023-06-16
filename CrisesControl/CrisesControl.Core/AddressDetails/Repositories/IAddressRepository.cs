using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.AddressDetails.Repositories
{
    public interface IAddressRepository
    {
        Task<int> AddAddress(Address address);
        Task<int> UpdateAddress(Address address);
        Task<Address> GetAddress(int AddressId);
        Task<List<Address>> GetAllAddress(int PageNumber, int PageSize, string OrderBy);
        Task<bool> DeleteAddress(int AddressId);

    }
}
