﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    public interface IJsonNormalizationBehavior
    {
        IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json);
    }
}