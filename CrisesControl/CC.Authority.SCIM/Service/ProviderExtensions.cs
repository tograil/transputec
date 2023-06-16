//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using CC.Authority.SCIM.Protocol;

namespace CC.Authority.SCIM.Service
{
    public static class ProviderExtension
    {
        public static IReadOnlyCollection<IExtension> ReadExtensions(this IProvider provider)
        {
            if(null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            IReadOnlyCollection<IExtension> result;
            try
            {
                result = provider.Extensions;
            }
            catch (NotImplementedException)
            {
                result = null;
            }
            return result;
        }
    }

}
