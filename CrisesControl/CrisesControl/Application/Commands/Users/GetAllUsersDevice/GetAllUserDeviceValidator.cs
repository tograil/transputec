﻿using CrisesControl.Core.Users;
using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice
{
    public class GetAllUserDeviceValidator:AbstractValidator<GetAllUserDevicesRequest>
    {
        public GetAllUserDeviceValidator()
        {
            RuleFor(x => x.OutUserCompanyId).GreaterThan(0);
            RuleFor(x => x.UserID).GreaterThan(0);
        }
    }
}
