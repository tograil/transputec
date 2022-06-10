//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Protocol
{
    [DataContract]
    public sealed class QueryResponse<TResource> : QueryResponseBase<TResource>
        where TResource : Resource
    {
        public QueryResponse()
            : base(ProtocolSchemaIdentifiers.Version2ListResponse)
        {
        }

        public QueryResponse(IReadOnlyCollection<TResource> resources)
            : base(ProtocolSchemaIdentifiers.Version2ListResponse, resources)
        {
        }

        public QueryResponse(IList<TResource> resources)
            : base(ProtocolSchemaIdentifiers.Version2ListResponse, resources)
        {
        }
    }
}