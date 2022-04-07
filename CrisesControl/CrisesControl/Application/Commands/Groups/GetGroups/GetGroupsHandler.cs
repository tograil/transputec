using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroups
{
    public class GetGroupsHandler: IRequestHandler<GetGroupsRequest, GetGroupsResponse>
    {
        private readonly GetGroupsValidator _groupValidator;
        private readonly IGroupQuery _groupQuery;
        private readonly IMapper _mapper;

        public GetGroupsHandler(GetGroupsValidator groupValidator, IGroupQuery groupQuery, IMapper mapper)
        {
            _groupValidator = groupValidator;
            _groupQuery = groupQuery;
            _mapper = mapper;
        }

        public async Task<GetGroupsResponse> Handle(GetGroupsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetGroupsRequest));
            
            await _groupValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _groupQuery.GetGroups(request);
            return departments;
        }
    }
}
