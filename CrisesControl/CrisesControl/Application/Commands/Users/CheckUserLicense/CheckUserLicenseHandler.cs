using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CheckUserLicense
{
    public class CheckUserLicenseHandler : IRequestHandler<CheckUserLicenseRequest, CheckUserLicenseResponse>
    {
        private readonly CheckUserLicenseValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CheckUserLicenseHandler> _logger;

        public CheckUserLicenseHandler(CheckUserLicenseValidator userValidator, IUserRepository userService, ILogger<CheckUserLicenseHandler> logger)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
        }

        public async Task<CheckUserLicenseResponse> Handle(CheckUserLicenseRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckUserLicenseRequest));
            //var mappedRequest = _mapper.Map<BulkActionModel>(request);
            var result = await _userRepository.CheckUserLicense(request.SessionId,request.UserList,request.CompanyId, request.UserId);
            return new CheckUserLicenseResponse();
        }
    }
}
