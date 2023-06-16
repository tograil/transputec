//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    public interface ISchematizedJsonDeserializingFactory<TOutput> where TOutput : Schematized
    {
        TOutput Create(IReadOnlyDictionary<string, object> json);
    }
}