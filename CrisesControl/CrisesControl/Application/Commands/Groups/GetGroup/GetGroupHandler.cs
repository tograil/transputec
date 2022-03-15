using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupHandler: IRequestHandler<GetGroupRequest, GetGroupResponse>
    {
        private readonly GetGroupValidator _groupValidator;
        private readonly IGroupQuery _groupQuery;

        public GetGroupHandler(GetGroupValidator groupValidator, IGroupQuery groupQuery)
        {
            _groupValidator = groupValidator;
            _groupQuery = groupQuery;
        }

        public async Task<GetGroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetGroupRequest));
            
            await _groupValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var groups = await _groupQuery.GetGroup(request);

            return groups;
        }
    }
}
