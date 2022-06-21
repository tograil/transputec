// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public sealed class DeletionRequest :
        SystemForCrossDomainIdentityManagementRequest<IResourceIdentifier>
    {
        public DeletionRequest(
            HttpRequestMessage request,
            IResourceIdentifier payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
