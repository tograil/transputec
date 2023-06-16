//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public interface IPaginationParameters
    {
        int? Count { get; set; }
        int? StartIndex { get; set; }
    }
}