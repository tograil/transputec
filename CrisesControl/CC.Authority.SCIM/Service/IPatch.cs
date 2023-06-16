// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public interface IPatch
    {
        PatchRequestBase PatchRequest { get; set; }
        IResourceIdentifier ResourceIdentifier { get; set; }
    }
}
