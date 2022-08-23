using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.GetGroupUsers
{
    public class GetGroupUsersHandler : IRequestHandler<GetGroupUsersRequest, GetGroupUsersResponse>
    {
        private readonly GetGroupUsersValidator _getGroupUsersValidator;
        private readonly ICompanyQuery _companyQuery;
        private readonly ILogger<GetGroupUsersHandler> _logger;

        public GetGroupUsersHandler(GetGroupUsersValidator getGroupUsersValidator, ICompanyQuery companyQuery, ILogger<GetGroupUsersHandler> logger)
        {
            _getGroupUsersValidator = getGroupUsersValidator;
            _companyQuery = companyQuery;
            _logger = logger;
        }
        public async Task<GetGroupUsersResponse> Handle(GetGroupUsersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetGroupUsersRequest));

            await _getGroupUsersValidator.ValidateAndThrowAsync(request, cancellationToken);

            var companyInfo = await _companyQuery.GetGroupUsers(request);

            return companyInfo;
        }
    }
}
