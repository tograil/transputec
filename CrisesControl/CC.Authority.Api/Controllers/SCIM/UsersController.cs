// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using CC.Authority.SCIM.Service.Monitor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

namespace CC.Authority.Api.Controllers.SCIM
{
    [Route(ServiceConstants.RouteUsers)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ApiController]
    public sealed class UsersController : ControllerTemplate<Core2EnterpriseUser>
    {
        public UsersController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }
    }
}
