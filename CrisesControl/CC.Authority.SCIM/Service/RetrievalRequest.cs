// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public sealed class RetrievalRequest :
        SystemForCrossDomainIdentityManagementRequest<IResourceRetrievalParameters>
    {
        public RetrievalRequest(
            HttpRequestMessage request,
            IResourceRetrievalParameters payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
