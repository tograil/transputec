using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.TempRegister
{
    public class TempRegisterHandler : IRequestHandler<TempRegisterRequest, TempRegisterResponse>
    {
        private readonly ILogger<TempRegisterHandler> _logger;
        private readonly IRegisterQuery _registerQuery;
        private readonly TempRegisterValidator _tempRegisterValidator;
        public TempRegisterHandler(IRegisterQuery registerQuery, ILogger<TempRegisterHandler> logger, TempRegisterValidator tempRegisterValidator)
        {
          this._registerQuery = registerQuery;
          this._logger = logger;
          this._tempRegisterValidator=tempRegisterValidator;
        }
        public async Task<TempRegisterResponse> Handle(TempRegisterRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(TempRegisterRequest));

            await _tempRegisterValidator.ValidateAndThrowAsync(request, cancellationToken);

            var tempReg = await _registerQuery.TempRegister(request);

            return tempReg;
        }
    }
}
