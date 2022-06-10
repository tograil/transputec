﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Enum of type names will contain type names")]
    public enum AttributeDataType
    {
        @string,
        boolean,
        @decimal,
        integer,
        dateTime,
        binary,
        reference,
        complex
    }
}
