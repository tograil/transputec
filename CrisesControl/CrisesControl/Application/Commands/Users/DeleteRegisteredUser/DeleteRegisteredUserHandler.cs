using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteRegisteredUser
{
    public class DeleteRegisteredUserHandler : IRequestHandler<DeleteRegisteredUserRequest, DeleteRegisteredUserResponse>
    {
        private readonly DeleteRegisteredUserValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mappper;

        public DeleteRegisteredUserHandler(DeleteRegisteredUserValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mappper = mapper;
        }

        public async Task<DeleteRegisteredUserResponse> Handle(DeleteRegisteredUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteRegisteredUserRequest));

            User value = _mappper.Map<DeleteRegisteredUserRequest, User>(request);
            var user = await _userRepository.DeleteRegisteredUser(request.CustomerId,request.UniqueGuiId, cancellationToken);
            var result = new DeleteRegisteredUserResponse();
            //result.UserId = user.UserId;
            return result;
        }
    }
}
