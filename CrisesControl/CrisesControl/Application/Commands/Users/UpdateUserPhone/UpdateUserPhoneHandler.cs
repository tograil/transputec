﻿using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUserPhone
{
    public class UpdateUserPhoneHandler: IRequestHandler<UpdateUserPhoneRequest, UpdateUserPhoneResponse>
    {
        private readonly UpdateUserPhoneValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserPhoneHandler> _logger;
        private readonly ICurrentUser _currentUser;


        public UpdateUserPhoneHandler(UpdateUserPhoneValidator userValidator, IUserRepository userService, ILogger<UpdateUserPhoneHandler> logger, ICurrentUser currentUser)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<UpdateUserPhoneResponse> Handle(UpdateUserPhoneRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateUserPhoneRequest));

            User value= new User()
            {
                MobileNo=request.MobileNo,
                Isdcode=request.MobileISDCode
            };
            if (CheckDuplicate(value))
            {
                var user = await _userRepository.UpdateUserPhone(value, request.MobileNo, request.MobileISDCode, cancellationToken);
                var result = new UpdateUserPhoneResponse();
                result.UserId = user.UserId;   
                return result;
            } 
            throw new UserNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
        }

        private bool CheckDuplicate(User user)
        {
            return _userRepository.CheckDuplicate(user);
        }
    }
}
