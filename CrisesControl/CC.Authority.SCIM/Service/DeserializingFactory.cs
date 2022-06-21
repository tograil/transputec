// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Schemas;

namespace CC.Authority.SCIM.Service
{
    public abstract class DeserializingFactory<TResource> :
            JsonDeserializingFactory<TResource>,
            IResourceJsonDeserializingFactory<TResource> where TResource : Resource, new()
    {
        public new TResource Create(IReadOnlyDictionary<string, object> json)
        {
            TResource result = base.Create(json);
            return result;
        }
    }
}
