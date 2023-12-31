// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using CC.Authority.SCIM.Service.Monitor;
using Microsoft.AspNetCore.Mvc;

namespace CC.Authority.Api.Controllers.SCIM
{
    [Route("/scim/root")]
    [ApiController]
    public sealed class RootController : ControllerTemplate<Resource>
    {
        public RootController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Resource> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Resource> result = new RootProviderAdapter(provider);
            return result;
        }
    }
}
