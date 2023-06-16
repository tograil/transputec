//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace CC.Authority.SCIM.Protocol
{
    [DataContract]
    public sealed class BulkResponse2 : BulkOperations<BulkResponseOperation>
    {
        public BulkResponse2()
            : base(ProtocolSchemaIdentifiers.Version2BulkResponse)
        {
        }
    }
}
