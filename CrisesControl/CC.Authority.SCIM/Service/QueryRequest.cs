// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public sealed class QueryRequest :
        SystemForCrossDomainIdentityManagementRequest<IQueryParameters>
    {
        public QueryRequest(
            HttpRequestMessage request,
            IQueryParameters payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}
