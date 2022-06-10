//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CC.Authority.SCIM.Protocol
{
    public sealed class ResourceRetrievalParameters : RetrievalParameters, IResourceRetrievalParameters
    {
        public ResourceRetrievalParameters(
            string schemaIdentifier,
            string path,
            string resourceIdentifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            if (null == resourceIdentifier)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            this.ResourceIdentifier =
                new ResourceIdentifier()
                {
                    Identifier = resourceIdentifier,
                    SchemaIdentifier = this.SchemaIdentifier
                };
        }

        public ResourceRetrievalParameters(
            string schemaIdentifier,
            string path,
            string resourceIdentifier)
            : base(schemaIdentifier, path)
        {
            if (null == resourceIdentifier)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            this.ResourceIdentifier =
                new ResourceIdentifier()
                {
                    Identifier = resourceIdentifier,
                    SchemaIdentifier = this.SchemaIdentifier
                };
        }

        public IResourceIdentifier ResourceIdentifier
        {
            get;
            private set;
        }
    }
}