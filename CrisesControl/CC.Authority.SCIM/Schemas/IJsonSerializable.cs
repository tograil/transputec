//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    public interface IJsonSerializable
    {
        Dictionary<string, object> ToJson();
        string Serialize();

    }
}
