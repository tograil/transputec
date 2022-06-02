using CrisesControl.Api.Application.Commands.Register.CheckCustomer;

namespace CrisesControl.Api.Application.Query
{
    public interface IRegisterQuery
    {
        Task<CheckCustomerResponse> CheckCustomer(CheckCustomerRequest request);
    }
}
