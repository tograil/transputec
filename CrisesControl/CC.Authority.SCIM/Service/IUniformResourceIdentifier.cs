// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public interface IUniformResourceIdentifier
    {
        bool IsQuery { get; }

        IResourceIdentifier Identifier { get; }
        IResourceQuery Query { get; }
    }
}
