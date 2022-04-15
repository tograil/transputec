using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ValidateEmail
{
    public class ValidateEmailHandler: IRequestHandler<ValidateEmailRequest, ValidateEmailResponse>
    {
        private readonly ValidateEmailValidator _userValidator;
        private readonly IUserQuery _userQuery;
        private readonly IMapper _mapper;

        public ValidateEmailHandler(ValidateEmailValidator userValidator, IUserQuery userQuery, IMapper mapper)
        {
            _userValidator = userValidator;
            _userQuery = userQuery;
            _mapper = mapper;
        }

        public async Task<ValidateEmailResponse> Handle(ValidateEmailRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ValidateEmailRequest));
            var response = await _userQuery.ValidateLoginEmail(request);
            return response;

        }
    }
}
