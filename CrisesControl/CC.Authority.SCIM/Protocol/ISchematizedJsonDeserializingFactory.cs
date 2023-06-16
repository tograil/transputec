//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    internal interface ISchematizedJsonDeserializingFactory :
        IGroupDeserializer,
        IPatchRequest2Deserializer,
        IUserDeserializer
    {
    }
}