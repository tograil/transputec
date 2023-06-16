//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Schemas
{
    public interface IChange
    {
        string Watermark { get; set; }
    }
}