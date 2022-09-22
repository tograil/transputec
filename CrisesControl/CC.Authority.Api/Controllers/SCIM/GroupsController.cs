// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using CC.Authority.SCIM.Service.Monitor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace CC.Authority.Api.Controllers.SCIM
{
    [Route(ServiceConstants.RouteGroups)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ApiController]
    public sealed class GroupsController : ControllerTemplate<Core2Group>
    {
        public GroupsController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Core2Group> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2Group> result =
                new Core2GroupProviderAdapter(provider);
            return result;
        }
    }
}
