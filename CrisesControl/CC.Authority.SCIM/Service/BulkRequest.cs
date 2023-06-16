//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public sealed class BulkRequest : SystemForCrossDomainIdentityManagementRequest<BulkRequest2>
    {
        public BulkRequest(
            HttpRequestMessage request,
            BulkRequest2 payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
