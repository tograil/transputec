// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public sealed class ReplaceRequest : SystemForCrossDomainIdentityManagementRequest<Resource>
    {
        public ReplaceRequest(
            HttpRequestMessage request,
            Resource payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
