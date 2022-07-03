using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.BulkAction
{
    public class BulkActionHandler : IRequestHandler<BulkActionRequest, BulkActionResponse>
    {
        private readonly BulkActionValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public BulkActionHandler(BulkActionValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<BulkActionResponse> Handle(BulkActionRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(BulkActionRequest));
            var mappedRequest = _mapper.Map<BulkActionModel>(request);
            var result = await _userRepository.BulkAction(mappedRequest, cancellationToken);
            return new BulkActionResponse();
        }
    }
}
