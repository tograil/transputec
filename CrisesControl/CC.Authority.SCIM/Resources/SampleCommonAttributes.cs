// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

using CC.Authority.SCIM.Schemas;

namespace Microsoft.SCIM.WebHostSample.Resources
{
    public static class SampleCommonAttributes
    {
        public static AttributeScheme IdentiFierAttributeScheme
        {
            get
            {
                AttributeScheme idScheme = new AttributeScheme("id", AttributeDataType.@string, false)
                {
                    Description = SampleConstants.DescriptionIdentifier
                };
                return idScheme;
            }
        }
    }
}
