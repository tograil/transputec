using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUserDevice
{
    public class DeleteUserDeviceHandler : IRequestHandler<DeleteUserDeviceRequest, DeleteUserDeviceResponse>
    {
        private readonly DeleteUserDeviceValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mappper;

        public DeleteUserDeviceHandler(DeleteUserDeviceValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mappper = mapper;
        }

        public async Task<DeleteUserDeviceResponse> Handle(DeleteUserDeviceRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteUserDeviceRequest));

            var isDeleted = await _userRepository.DeleteUserDevice(request.UserDeviceID, cancellationToken);
            var result = new DeleteUserDeviceResponse();
            result.IsDeleted = isDeleted;
            return result;
        }
    }
}
