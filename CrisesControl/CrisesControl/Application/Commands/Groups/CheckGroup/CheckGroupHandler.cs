using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CheckGroup
{
    public class CheckGroupHandler : IRequestHandler<CheckGroupRequest, CheckGroupResponse>
    {
        private readonly ILogger<CheckGroupHandler> _logger;
        private readonly CheckGroupValidator _checkGroupValidator;
        private readonly IGroupQuery _groupQuery;
        public CheckGroupHandler(ILogger<CheckGroupHandler> logger, CheckGroupValidator checkGroupValidator, IGroupQuery groupQuery)
        {
            this._logger = logger;
            this._checkGroupValidator = checkGroupValidator;
            this._groupQuery = groupQuery;
        }
        public async Task<CheckGroupResponse> Handle(CheckGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckGroupRequest));
            await _checkGroupValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _groupQuery.CheckGroup(request);
            return result;
        }
    }
}
