//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IExtension
    {
        Type Controller { get; }
        JsonDeserializingFactory JsonDeserializingFactory { get; }
        string Path { get; }
        string SchemaIdentifier { get; }
        string TypeName { get; }

        bool Supports(HttpRequestMessage request);
    }
}