using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CrisesControl.Api.Application.Commands.Users.CheckEmail
{
    public class CheckEmailHandler: IRequestHandler<CheckEmailRequest, CheckEmailResponse>
    {
        private readonly CheckEmailValidator _userValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CheckEmailHandler(CheckEmailValidator userValidator, IUserRepository userService, IMapper mapper)
        {
            _userValidator = userValidator;
            _userRepository = userService;
            _mapper = mapper;
        }

        public async Task<CheckEmailResponse> Handle(CheckEmailRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CheckEmailRequest));
            CheckEmailResponse response = new CheckEmailResponse();
           if(new EmailAddressAttribute().IsValid(request.EmailId))
            {
                if (await _userRepository.DuplicateEmail(request.EmailId))
                {
                    response.Message = "Email id already registered.";
                    response.IsExist = true;
                    return response;
                }

                if(await _userRepository.BadEmail(request.EmailId))
                {
                    response.Message = "You cannot use this email domain";
                    return response;
                }
                response.Message = "No record found.";
                response.IsExist = false;
                return response;
            } else
            {
                return null;
            }
        }

        private bool CheckDuplicate(User user)
        {
            return _userRepository.CheckDuplicate(user);
        }
    }
}
