using AutoMapper;
using CrisesControl.Api.Application.Commands.Addresses.AddAddress;
using CrisesControl.Api.Application.Commands.Addresses.DeleteAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAddress;
using CrisesControl.Api.Application.Commands.Addresses.GetAllAddress;
using CrisesControl.Api.Application.Commands.Addresses.UpdateAddress;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.AddressDetails.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class AddressQuery: IAddressQuery
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressQuery> _logger;
        private readonly ICurrentUser _currentUser;
        //private readonly IMapper _mapper;
        private readonly IPaging _paging;
        public AddressQuery(IAddressRepository addressRepository, 
            ILogger<AddressQuery> logger, ICurrentUser currentUser, IPaging paging)
        {
            this._addressRepository = addressRepository;
            this._logger = logger;
            this._currentUser = currentUser;
            //this._mapper = mapper;
            this._paging = paging;
        }

        public async Task<AddAddressResponse> AddAddress(AddAddressRequest request)
        {
            var address = new Address
            {
                AddressLabel=request.AddressLabel,
                AddressLine1=request.AddressLine1,
                AddressLine2=request.AddressLine2,
                AddressType=request.AddressType.ToDbString(),
                City=request.City,
                CountryCode=request.CountryCode,
                Postcode=request.Postcode,
                State=request.State,
                Status=1,
                CreatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                CreatedBy = _currentUser.UserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                UpdatedBy = _currentUser.UserId,
                
            };
            var AddId = await _addressRepository.AddAddress(address);
            //var result = _mapper.Map<Address>(address);
            var response = new AddAddressResponse();
            if (AddId > 0)
            {
                response.AddressId = AddId;
                response.Message = "Address added " + AddId;
            }
            else
            {
                response.AddressId = AddId;
                response.Message = "Address Not Added ";
            }
            return response;

        }

        public async Task<DeleteAddressResponse> DeleteAddress(DeleteAddressRequest request)
        {
            var response = new DeleteAddressResponse();
            if (request.AddressId>0)
            {
                var address=  await  _addressRepository.DeleteAddress(request.AddressId);
                //var result = _mapper.Map<bool>(address);

                response.Success = address;
                response.Message = "Deleted";

            }
            else
            {
                response.Success = false;
                response.Message = "No Data Found";
            }
            return response;
        }

        public async Task<GetAddressResponse> GetAdress(GetAddressRequest request)
        {
            var response = new GetAddressResponse();
            if (request.AddressId > 0)
            {
                var address = await _addressRepository.GetAddress(request.AddressId);
                //var result = _mapper.Map<Address>(address);

                response.data = address;
                response.Message = "Data has been loaded";

            }
            else
            {
                response.data = null;
                response.Message = "No Data Found";
            }
            return response;
        }

        public async Task<GetAllAddressResponse> GetAllAddress(GetAllAddressRequest request)
        {
            var address = await _addressRepository.GetAllAddress(_paging.PageNumber, _paging.PageSize, _paging.OrderBy);
            //var result = _mapper.Map<List<Address>>(address);
            var response = new GetAllAddressResponse();
            if (address != null)
            {
                response.Data = address;
                response.Message = "Data Loaded";
            }
            else
            {
                response.Data = address;
                response.Message = "No Data Found";
            }
            return response;
                    
        }

        public async Task<UpdateAddressResponse> UpdateAddress(UpdateAddressRequest request)
        {
            
            var address = new Address
            {
                AddressId=request.AddressId,
                AddressLabel = request.AddressLabel,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                AddressType = request.AddressType.ToDbString(),
                City = request.City,
                CountryCode = request.CountryCode,
                Postcode = request.Postcode,
                State = request.State,
                Status = 1,
                CreatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                CreatedBy = _currentUser.UserId,
                UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone),
                UpdatedBy = _currentUser.UserId,

            };
            var AddId = await _addressRepository.UpdateAddress(address);
            //var result = _mapper.Map<Address>(address);
            var response = new UpdateAddressResponse();
            if (AddId > 0)
            {
                response.AddressId = AddId;
                response.Message = "Address has been updated " + AddId;
            }
            else
            {
                response.AddressId = AddId;
                response.Message = "Address Not Updated ";
            }
            return response;
        }
    }
}
