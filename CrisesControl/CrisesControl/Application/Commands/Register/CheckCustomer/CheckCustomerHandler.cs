using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CheckCustomer
{
    public class CheckCustomerHandler : IRequestHandler<CheckCustomerRequest, CheckCustomerResponse>
    {
        private readonly ILogger<CheckCustomerHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly CheckCustomerValidator _checkCustomerValidator;
        public CheckCustomerHandler(IRegisterQuery registerQuery, ILogger<CheckCustomerHandler> logger, CheckCustomerValidator checkCustomerValidator)
        {
            this._registerQuery = registerQuery;
            this._logger = logger; 
            this._checkCustomerValidator=checkCustomerValidator;
        }  
        public async Task<CheckCustomerResponse> Handle(CheckCustomerRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckCustomerRequest));

            await _checkCustomerValidator.ValidateAndThrowAsync(request, cancellationToken);

            var customer = await _registerQuery.CheckCustomer(request);

            return customer;
        }
    }
}
