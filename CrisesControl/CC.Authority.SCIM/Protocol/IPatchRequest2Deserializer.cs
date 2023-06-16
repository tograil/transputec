//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Protocol
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializer", Justification = "False analysis")]
    public interface IPatchRequest2Deserializer
    {
        ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior { get; set; }
    }
}
