// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using System.Runtime.Serialization;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    [DataContract]
    public sealed class QueryResponse : QueryResponseBase
    {
        public QueryResponse()
            : base()
        {
        }

        public QueryResponse(IReadOnlyCollection<Resource> resources)
            : base(resources)
        {
        }

        public QueryResponse(IList<Resource> resources)
            : base(resources)
        {
        }
    }
}
