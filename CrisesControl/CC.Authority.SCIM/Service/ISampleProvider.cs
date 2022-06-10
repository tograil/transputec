// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public interface ISampleProvider
    {
        Core2Group SampleGroup { get; }
        PatchRequest2 SamplePatch { get; }
        Core2EnterpriseUser SampleResource { get; }
        Core2EnterpriseUser SampleUser { get; }
    }
}
