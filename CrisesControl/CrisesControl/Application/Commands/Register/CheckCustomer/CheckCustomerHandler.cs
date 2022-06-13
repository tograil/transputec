using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Register.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CheckCustomer
{
    public class CheckCustomerHandler : IRequestHandler<CheckCustomerRequest, CheckCustomerResponse>
    {
        private readonly ILogger<CheckCustomerHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly CheckCustomerValidator _checkCustomerValidator;
        private readonly IMapper _mapper;
        public CheckCustomerHandler(IRegisterQuery registerQuery, IMapper mapper,ILogger<CheckCustomerHandler> logger, CheckCustomerValidator checkCustomerValidator)
        {
            this._registerQuery = registerQuery;
           this._logger = logger; 
            this._checkCustomerValidator=checkCustomerValidator;
            this._mapper = mapper;
        }  
        public async Task<CheckCustomerResponse> Handle(CheckCustomerRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckCustomerRequest));

            await _checkCustomerValidator.ValidateAndThrowAsync(request, cancellationToken);

            var customer = await _registerQuery.CheckCustomer(request);
           if (customer != null)
            {
                customer.Message = "Customer ID already taken";
            }
            else
            {
                customer.Message = "No record found.";
            }


            return customer;
        }
    }
}
